using TaskList.Application.Configuration;

namespace TaskList.Api.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApiDI(this IServiceCollection services)
    {
        services.AddApplicationDI();

        return services;
    }
}
