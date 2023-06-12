namespace customer.data.Entities;

public class Customer : Entity
{
    public string Title { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTimeOffset Dob { get; set; }

    public long ContactNumber { get; set; }

    public string EmailAddress { get; set; } = null!;

    public Address Address { get; set; } = null!;

    public Guid ReferenceNumber { get; set; }

    public string GetFullName() => $"{Title} {FirstName} {LastName}";

    public string GetCompleteAddress() =>
        $"{Address.Street}, {Address.City}, {Address.Postcode}, {Address.Nation}, {Address.Country}";

    public string GetDateOfBirth() => Dob.ToString("D");

    public int GetAge()
    {
        var age = DateTime.Now.Year - Dob.Year;

        if (DateTime.Now.DayOfYear < Dob.DayOfYear)
        {
            age--;
        }

        return age;
    }
}
