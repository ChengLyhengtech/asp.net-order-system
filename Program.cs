using aps.net_order_system.Commands;
using aps.net_order_system.Commands.Create;
using aps.net_order_system.Commands.Delete;
using aps.net_order_system.Commands.Update;
using aps.net_order_system.Data;
using aps.net_order_system.Queries;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- ADD CORS POLICY HERE ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin() // Your Live Server address
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 1. Database Configuration (Example for SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Register All Handlers (CQRS)
builder.Services.AddScoped<GetCategoriesHandler>();
builder.Services.AddScoped<CreateCategoriesCommand>();
builder.Services.AddScoped<UpdateCategoriesHandler>();
builder.Services.AddScoped<DeleteCategoriesHandler>();

builder.Services.AddScoped<GetUsersHandler>();
builder.Services.AddScoped<CreateUserHandler>();
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
// -----------------------

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
app.UseAuthorization();
app.MapControllers();

app.Run();