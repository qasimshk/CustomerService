namespace customer.data.Entities;
public class Address : Entity
{
    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Postcode { get; set; } = null!;

    public string Nation { get; set; } = null!;

    public string Country { get; set; } = null!;

    public Customer Customer { get; set; } = null!;
}
