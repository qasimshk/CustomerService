namespace customer.api.Extensions;

using System.Net;
using System.Net.Http.Headers;
using Configurations;
using core.Abstractions.Mappers;
using core.Abstractions.Services;
using core.Mappers;
using core.Services;
using core.Validations;
using customer.core.Model;
using customer.core.Model.Results;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Middleware;

public static class ServiceDependenciesExtensions
{
    public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
    {
        services.AddControllers()
        .ConfigureApiBehaviorOptions(opt => opt.InvalidModelStateResponseFactory = context =>
            {
                var errors = new List<string>();

                foreach (var fields in context.ModelState.Keys)
                {
                    errors.AddRange(from error in context.ModelState[fields]?.Errors
                                    select error.ErrorMessage);
                }

                return new BadRequestObjectResult(Result.FailedResult(errors, HttpStatusCode.BadRequest));
            });

        //Paging
        services.AddScoped(typeof(IPagedList<>), typeof(PagedList<>));

        // Fluent Validator
        services.AddValidatorsFromAssemblyContaining<CustomerCreateValidator>();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();

        // Middlewares
        services.AddTransient<ExceptionHandlingMiddleware>();

        // Mappers
        services.AddScoped<ICustomerMapper, CustomerMapper>();

        // Services
        services.AddScoped<ICustomerService, CustomerService>();

        // HttpClient Services Configuration
        // https://www.milanjovanovic.tech/blog/the-right-way-to-use-httpclient-in-dotnet
        services.AddHttpClient<IPublicService, PublicService>((serviceProvider, client) =>
        {
            var settings = services.BuildServiceProvider().GetRequiredService<IOptions<ExternalServiceConfig>>().Value;

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.BaseAddress = new Uri(settings.ServiceUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

        return services;
    }
}
