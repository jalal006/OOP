using Backend.Api.Data;
using Backend.Api.Endpoints;
using Backend.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("AzureSql");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Azure SQL connection string was not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API v1");
    });
}

app.MapProductEndpoints();

app.Run();
