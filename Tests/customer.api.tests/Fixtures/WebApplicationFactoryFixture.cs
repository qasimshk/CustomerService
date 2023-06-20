namespace customer.api.tests.Fixtures;

using System.Net.Http;
using System.Threading.Tasks;
using customer.data.Context;
using customer.data.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;

public class WebApplicationFactoryFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;

    private const int InitialCustomerCount = 10;

    private MsSqlContainer _container;

    public WebApplicationFactoryFixture()
    {
        // Docker Container for Ms Sql
        _container = new MsSqlBuilder().Build();

        var connectionString = _container.GetConnectionString();

        // Web Application Factory
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<CustomerDbContext>>();

            services.AddDbContext<CustomerDbContext>(options => options.UseSqlServer(connectionString));
        }));

        Client = _factory.CreateDefaultClient();
    }

    public HttpClient Client { get; private set; }

    public async Task InitializeAsync()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var context = scopedServices.GetRequiredService<CustomerDbContext>();

            await context.Database.EnsureDeletedAsync();

            await context.Database.EnsureCreatedAsync();

            await context.Nations.AddRangeAsync(new List<Nation>
            {
                new Nation
                {
                    Id = 1,
                    Name = "England"
                },
                new Nation
                {
                    Id = 2,
                    Name = "Scotland"
                },
                new Nation
                {
                    Id = 3,
                    Name = "Wales"
                },
                new Nation
                {
                    Id = 4,
                    Name = "Northern Ireland"
                }
            });

            await context.Customers.AddRangeAsync(DataFixture.GetCustomerFaker(InitialCustomerCount));

            await context.Customers.AddRangeAsync(DataFixture.GetCustomerFixSeedingData());

            await context.SaveChangesAsync();
        }
    }

    public async Task DisposeAsync()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var context = scopedServices.GetRequiredService<CustomerDbContext>();

            await context.Database.EnsureDeletedAsync();
        }

        await _container.StopAsync();
    }
}
