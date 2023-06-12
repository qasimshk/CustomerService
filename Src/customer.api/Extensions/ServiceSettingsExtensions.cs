namespace customer.api.Extensions;

using Configurations;

public static class ServiceSettingsExtensions
{
    public static IServiceCollection AddServiceSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.Configure<ApplicationConfig>(configuration.GetSection(nameof(ApplicationConfig)));

        services.Configure<ExternalServiceConfig>(configuration.GetSection(nameof(ExternalServiceConfig)));

        return services;
    }
}
