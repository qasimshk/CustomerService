namespace customer.api.tests.Fixtures;

using Bogus;
using customer.data.Entities;

public static class DataFixture
{
    public static List<Customer> GetCustomerFaker(int numberOfRecords)
    {
        var fakeCustomer = new Faker<Customer>()
            .RuleFor(c => c.Title, "Mr")
            .RuleFor(c => c.FirstName, (faker, t) => faker.Name.FirstName())
            .RuleFor(c => c.LastName, (faker, t) => faker.Name.LastName())
            .RuleFor(c => c.ContactNumber, 123456789)
            .RuleFor(c => c.Dob, (faker, t) => faker.Person.DateOfBirth)
            .RuleFor(e => e.EmailAddress, (faker, t) => faker.Internet.Email(t.FirstName, t.LastName))
            .RuleFor(c => c.ReferenceNumber, (_, _) => Guid.NewGuid())
            .RuleFor(c => c.Address, (faker, t) => GetCustomerAddress(t).Generate(1).First());

        return fakeCustomer.Generate(numberOfRecords).ToList();
    }

    public static List<Customer> GetCustomerFixSeedingData() => new()
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
            EmailAddress = "andrea.pedersen@example.com",
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

    private static Faker<Address> GetCustomerAddress(Customer customer) => new Faker<Address>()
        .RuleFor(x => x.Street, (faker, t) => faker.Address.StreetAddress())
        .RuleFor(x => x.City, (faker, t) => faker.Address.City())
        .RuleFor(x => x.Country, (faker, t) => faker.Address.Country())
        .RuleFor(x => x.Nation, "England")
        .RuleFor(x => x.Postcode, "WE2 8HD")
        .RuleFor(x => x.Customer, customer);
}
