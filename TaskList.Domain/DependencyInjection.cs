using Microsoft.Extensions.DependencyInjection;

namespace TaskList.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainDI(this IServiceCollection services)
    {
        return services;
    }
}
