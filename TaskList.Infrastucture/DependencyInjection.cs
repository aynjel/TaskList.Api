using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskList.Infrastucture.Persistence;

namespace TaskList.Infrastucture;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(connectionString));

        return services;
    }
}
