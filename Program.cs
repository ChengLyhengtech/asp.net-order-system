
using aps.net_order_system.Commands;
using aps.net_order_system.Commands.Create;
using aps.net_order_system.Commands.Delete;
using aps.net_order_system.Commands.Update;
using aps.net_order_system.Data;
using aps.net_order_system.Interface;
using aps.net_order_system.Models;
using aps.net_order_system.Queries;
using aps.net_order_system.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


// --- ADD CORS POLICY HERE ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(origin => true) // Allow any origin
              .AllowCredentials();
    });
});

//By default, the Swagger UI doesn't know how to send your JWT token to the backend.
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Order System API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// 1. Database Configuration (Example for SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// ASP.NET Core Identity Setup
builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


// JWT Setup
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});

// In Program.cs
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This allows the API to match "expiryDate" to "ExpiryDate"
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// QRCode 
builder.Services.AddDataProtection();
builder.Services.AddScoped<ITableQrService, TableQrService>();
//KHQR
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddHttpClient<IPaymentService, PaymentService>();

// 2. Register All Handlers (CQRS)
builder.Services.AddScoped<GetCategoriesHandler>();
builder.Services.AddScoped<CreateCategoriesCommand>();
builder.Services.AddScoped<UpdateCategoriesHandler>();
builder.Services.AddScoped<DeleteCategoriesHandler>();

builder.Services.AddScoped<GetUsersHandler>();
//builder.Services.AddScoped<CreateUserHandler>();
builder.Services.AddScoped<UpdateUserHandler>();
builder.Services.AddScoped<DeleteUserHandler>();

builder.Services.AddScoped<GetProductHandler>();
builder.Services.AddScoped<CreateProductCommand>();
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<DeleteProductHandler>();
builder.Services.AddScoped<GetTopProductHandler>();

// --- Add these lines ---
builder.Services.AddScoped<GetAllOrdersQueryHandler>();
builder.Services.AddScoped<GetOrderQueryHandler>();
builder.Services.AddScoped<CreateOrderCommandHandler>();
builder.Services.AddScoped<UpdateOrderStatusCommandHandler>();
builder.Services.AddScoped<DeleteOrderCommandHandler>();
builder.Services.AddScoped<ITokenService, TokenService>();
// -----------------------

builder.Services.AddScoped<TotalCountOrderHandler>();

// 3. Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

    

app.UseCors("AllowFrontend");

// 4. Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // <--- ADD THIS LINE HERE
app.UseAuthorization();
app.MapControllers();


//Before you can register a user with a role, those roles must exist in your database.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Staff", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

app.Run();