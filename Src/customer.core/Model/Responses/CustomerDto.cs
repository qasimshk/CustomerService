namespace customer.core.Model.Responses;

public class CustomerDto
{
    public string FullName { get; set; } = null!;

    public string DateOfBirth { get; set; } = null!;

    public int Age { get; set; }

    public long ContactNumber { get; set; }

    public string EmailAddress { get; set; } = null!;

    public Guid RefrenceNumber { get; set; }

    public string CompleteAddress { get; set; } = null!;
}
