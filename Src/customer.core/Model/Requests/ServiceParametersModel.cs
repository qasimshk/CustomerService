namespace customer.core.Model.Requests;

public class ServiceParametersModel : QueryStringParameters
{
    public string? Api { get; set; }

    public string? Category { get; set; }
}
