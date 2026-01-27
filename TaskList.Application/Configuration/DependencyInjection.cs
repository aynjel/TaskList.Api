using Microsoft.Extensions.DependencyInjection;

namespace TaskList.Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDI(this IServiceCollection services)
    {
        return services;
    }
}
