using TaskList.Application;

namespace TaskList.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiDI(this IServiceCollection services)
    {
        services.AddApplicationDI();

        return services;
    }
}
