namespace customer.api.tests.TestData;

using customer.core.Model.Requests;

public class CustomerCreateModelValidTestData : TheoryData<CustomerCreateModel>
{
    public CustomerCreateModelValidTestData()
    {
        Add(new CustomerCreateModel
        {
            Title = "Mr",
            FirstName = "Abby",
            LastName = "Will",
            ContactNumber = 123456789,
            Dob = DateTimeOffset.Now,
            EmailAddress = "abby@test.com",
            ReferenceNumber = Guid.NewGuid(),
            Address = new CustomerAddress
            {
                Street = "test street",
                City = "Test",
                Country = "Test",
                Nation = "England",
                Postcode = "SE10 9QR"
            }
        });

        Add(new CustomerCreateModel
        {
            Title = "Ms",
            FirstName = "Emma",
            LastName = "Reilly",
            ContactNumber = 123456789,
            Dob = DateTimeOffset.Now,
            EmailAddress = "example@test.com",
            ReferenceNumber = Guid.NewGuid(),
            Address = new CustomerAddress
            {
                Street = "test street",
                City = "Test",
                Country = "Test",
                Nation = "England",
                Postcode = "SE10 9QR"
            }
        });
    }
}
