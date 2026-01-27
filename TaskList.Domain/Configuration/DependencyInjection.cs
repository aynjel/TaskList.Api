using Microsoft.Extensions.DependencyInjection;

namespace TaskList.Domain.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainDI(this IServiceCollection services)
    {
        return services;
    }
}
