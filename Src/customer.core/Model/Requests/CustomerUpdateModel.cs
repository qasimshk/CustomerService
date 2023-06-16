namespace customer.core.Model.Requests;

public class CustomerUpdateModel
{
    public string? Title { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTimeOffset? Dob { get; set; }

    public long? ContactNumber { get; set; }
}
