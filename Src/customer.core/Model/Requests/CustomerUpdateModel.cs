namespace customer.core.Model.Requests;

public class CustomerUpdateModel
{
    public string Title { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTimeOffset Dob { get; set; }

    public long ContactNumber { get; set; }
}
