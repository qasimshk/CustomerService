namespace customer.core.Model.Requests;
public class CustomerCreateModel
{
    public CustomerCreateModel() => Address = new CustomerAddress();

    public string Title { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTimeOffset Dob { get; set; }

    public long ContactNumber { get; set; }

    public string EmailAddress { get; set; } = null!;

    public Guid ReferenceNumber { get; set; }

    public CustomerAddress Address { get; set; }
}

public class CustomerAddress
{
    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Postcode { get; set; } = null!;

    public string Nation { get; set; } = null!;

    public string Country { get; set; } = null!;
}
