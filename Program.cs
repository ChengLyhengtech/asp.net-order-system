using aps.net_order_system.Commands;
using aps.net_order_system.Commands.Create;
using aps.net_order_system.Commands.Delete;
using aps.net_order_system.Commands.Update;
using aps.net_order_system.Data;
using aps.net_order_system.Hubs;
using aps.net_order_system.Interface;
using aps.net_order_system.Models;
using aps.net_order_system.Queries;
using aps.net_order_system.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


// 2. Make sure your CORS policy allows SignalR (It needs AllowCredentials)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Replace with your exact React URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // CRITICAL FOR SIGNALR WEBSOCKETS
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
})
.AddJwtBearer(options => {
    // Inside Program.cs -> AddJwtBearer
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),

        // REMOVE OR COMMENT OUT THESE TWO LINES if using ClaimTypes.Role:
        // RoleClaimType = "role",     
        // NameClaimType = "nameid"    
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
builder.Services.AddScoped<GetCategoryByIdHandler>();

builder.Services.AddScoped<GetUsersHandler>();
//builder.Services.AddScoped<CreateUserHandler>();
builder.Services.AddScoped<UpdateUserHandler>();
builder.Services.AddScoped<DeleteUserHandler>();

builder.Services.AddScoped<GetProductHandler>();
// Register your new handler in the DI container
builder.Services.AddScoped<GetProductByIdHandler>();
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
builder.Services.AddScoped<GetStaffHistoryHandler>();
builder.Services.AddScoped<GetSalesAnalyticsHandler>();
builder.Services.AddScoped<GetAdminOrderHistoryHandler>();
// -----------------------

builder.Services.AddScoped<TotalCountOrderHandler>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// 3. Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Somewhere near your payment services or token services:
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen();
// 1. Add this near your other builder.Services definitions
builder.Services.AddSignalR();

var app = builder.Build();
// 3. Map the Hub endpoint down near app.MapControllers()
app.MapHub<OrderHub>("/orderHub");

app.UseStaticFiles(); // Enables files in wwwroot

// Manually expose the 'uploads' folder
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});

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


// Inside Program.cs - Replace your role creation block with this:

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

    // 1. Ensure Roles Exist
    string[] roles = { "Admin", "Staff", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // 2. Seed Initial System Admin
    string adminUsername = "SuperAdmin";
    string adminEmail = "SuperAdmin@system.com";

    var adminUser = await userManager.FindByNameAsync(adminUsername);
    if (adminUser == null)
    {
        var newAdmin = new UserModel
        {
            UserName = adminUsername,
            Email = adminEmail,
            FullName = "System Administrator",
            EmailConfirmed = true
        };

        // Set your highly secure initial password here
        var createAdminResult = await userManager.CreateAsync(newAdmin, "Admin@123");

        if (createAdminResult.Succeeded)
        {
            // Assign the Admin role
            await userManager.AddToRoleAsync(newAdmin, "Admin");
            Console.WriteLine("--> Master Admin Account seeded successfully.");
        }
        else
        {
            Console.WriteLine("--> Failed to seed Admin account:");
            foreach (var error in createAdminResult.Errors)
            {
                Console.WriteLine($"   Error: {error.Description}");
            }
        }
    }
}

app.Run();