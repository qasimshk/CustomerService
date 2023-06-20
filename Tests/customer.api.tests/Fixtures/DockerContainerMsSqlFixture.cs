namespace customer.api.tests.Fixtures;

using System.Threading.Tasks;
using customer.data.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using customer.data.Entities;

public class DockerContainerMsSqlFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    // https://dotnet.testcontainers.org/
    // https://github.com/bchavez/Bogus

    private readonly MsSqlContainer _container;

    private const int InitialCustomerCount = 10;

    public DockerContainerMsSqlFixture() => _container = new MsSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _container.GetConnectionString();

        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<CustomerDbContext>>();

            services.AddDbContext<CustomerDbContext>(options => options.UseSqlServer(connectionString));
        });

        builder.UseEnvironment("Test");
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        using (var scope = Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var context = scopedServices.GetRequiredService<CustomerDbContext>();

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

    public async Task DisposeAsync() => await _container.StopAsync();
}
