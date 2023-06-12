namespace customer.api.Extensions;

using Health;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddServiceHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("Database");

        return services;
    }
}
