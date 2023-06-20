namespace customer.api;

using System.Reflection;
using data.Context;
using data.Entities;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Middleware;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // this will avoid the mistake of committing settings in origin
        builder.Configuration.AddEnvironmentVariables()
                     .AddUserSecrets(Assembly.GetExecutingAssembly(), true);

        builder.Services
            .AddDbContext<CustomerDbContext>(opt => opt.UseInMemoryDatabase("CustomerRecords"))
            .AddServiceSettings(builder.Configuration)
            .AddServiceDependencies()
            .AddServiceHealthCheck();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            ///Seeding in-memory database
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();

                await SeedingDatabase(dbContext!);
            }
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapHealthChecks("/health");

        app.MapControllers();

        app.Run();
    }

    # region Seeding database

    private static async Task SeedingDatabase(CustomerDbContext customerDbContext)
    {
        if (!await customerDbContext.Customers.AnyAsync())
        {
            await customerDbContext.Customers.AddRangeAsync(GetCustomerSeedingData());
        }

        if (!await customerDbContext.Nations.AnyAsync())
        {
            await customerDbContext.Nations.AddRangeAsync(new List<Nation>
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
        }

        await customerDbContext.SaveChangesAsync();
    }

    private static List<Customer> GetCustomerSeedingData() => new()
    {
        new Customer
        {
            Title = "Ms",
            FirstName = "Olesya",
            LastName = "Kiyak",
            Dob = DateTime.Parse("1953-09-15"),
            ContactNumber = 09645634478,
            EmailAddress = "olesya.kiyak@example.com",
            ReferenceNumber = Guid.Parse("96d0baa3-55f0-4d09-8ffa-f32d27dc4870"),
            Address = new Address
            {
                Street = "6940 Pavlenka",
                City = "Pereyaslav",
                Nation = "England",
                Postcode = "41944",
                Country = "UK"
            }
        },
        new Customer
        {
            Title = "Mr",
            FirstName = "Tromp",
            LastName = "Verwaaijen",
            Dob = DateTime.Parse("1979-11-16"),
            ContactNumber = 06545634479,
            EmailAddress = "tromp.verwaaijen@example.com",
            ReferenceNumber = Guid.Parse("ee39b52b-25aa-432b-a694-f92ff593029a"),
            Address = new Address
            {
                Street = "5641 Hoge Naarderweg",
                City = "Hoeven",
                Nation = "Scotland",
                Postcode = "4168 MK",
                Country = "UK"
            }
        },
        new Customer
        {
            Title = "Miss",
            FirstName = "Mavka",
            LastName = "Krikunenko",
            Dob = DateTime.Parse("1999-10-17"),
            ContactNumber = 01145634335,
            EmailAddress = "mavka.krikunenko@example.com",
            ReferenceNumber = Guid.Parse("77eb8dcc-a3d9-4cbe-8342-0ffd4078f7e8"),
            Address = new Address
            {
                Street = "5324 Tagilskiy provulok",
                City = "Snigurivka",
                Nation = "Wales",
                Postcode = "67013",
                Country = "UK"
            }
        },
        new Customer
        {
            Title = "Mr",
            FirstName = "Mille",
            LastName = "Pedersen",
            Dob = DateTime.Parse("1952-11-30"),
            ContactNumber = 01145634335,
            EmailAddress = "mille.pedersen@example.com",
            ReferenceNumber = Guid.Parse("ab719a26-7c52-49f5-babf-92c8f5308ba1"),
            Address = new Address
            {
                Street = "899 Plantagevej",
                City = "Ryslinge",
                Nation = "Northern Ireland",
                Postcode = "48450",
                Country = "UK"
            }
        },
        new Customer
        {
            Title = "Miss",
            FirstName = "Andrea",
            LastName = "Petersen",
            Dob = DateTime.Parse("1984-11-30"),
            ContactNumber = 01145634335,
            EmailAddress = "Andrea.pedersen@example.com",
            ReferenceNumber = Guid.Parse("60ec1279-d5b2-41c8-8488-4825d509f8c2"),
            Address = new Address
            {
                Street = "215 Anlægsvej",
                City = "Stoevring",
                Nation = "Wales",
                Postcode = "10229",
                Country = "UK"
            }
        }
    };

    #endregion
}
