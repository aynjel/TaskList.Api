using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskList.Application.Common;
using TaskList.Application.Interfaces;
using TaskList.Infrastucture.Indentity;
using TaskList.Infrastucture.Persistence;
using TaskList.Infrastucture.Persistence.Repositories;
using TaskList.Infrastucture.Services;

namespace TaskList.Infrastucture.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(connectionString));

        // Configure JWT Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        // Configure AI Settings
        services.Configure<AiSettings>(configuration.GetSection("AiSettings"));

        // Configure Identity
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        // Register services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        
        // Register AI services based on ServiceType
        var aiSettings = configuration.GetSection("AiSettings");
        var serviceType = aiSettings.GetValue<string>("ServiceType") ?? "AzureOpenAI";
        
        if (serviceType.Equals("GoogleGemini", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IAiService, GoogleGeminiAiService>();
            Console.WriteLine($"[AI Provider] Registered: Google Gemini (Model: {aiSettings.GetValue<string>("ModelId") ?? "gemini-2.0-flash-exp"})");
        }
        else
        {
            services.AddScoped<IAiService, AiSummaryService>();
            Console.WriteLine($"[AI Provider] Registered: Azure OpenAI (Deployment: {aiSettings.GetValue<string>("DeploymentName")})");
        }
        
        services.AddScoped<IDocumentParserService, DocumentParserService>();
        
        // Register repositories
        services.AddScoped<ITaskRepository, TaskRepository>();
        
        // Register domain services
        services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}
