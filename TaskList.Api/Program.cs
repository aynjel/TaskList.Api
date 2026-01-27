using Scalar.AspNetCore;
using TaskList.Api;
using TaskList.Infrastucture;
using TaskList.Infrastucture.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add OpenAPI services
builder.Services.AddOpenApi();

builder.Services.AddApiDI();
builder.Services.AddInfrastructureDI(builder.Configuration);

var app = builder.Build();

// Initialize database (apply migrations if needed)
await app.InitializeDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Map OpenAPI endpoint
    app.MapOpenApi();
    
    // Add Scalar API documentation UI
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("TaskList API")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
