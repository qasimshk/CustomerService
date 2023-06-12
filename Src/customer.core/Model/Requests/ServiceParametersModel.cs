namespace customer.core.Model.Requests;

public class ServiceParametersModel : QueryStringParameters
{
    public string Api { get; set; } = null!;

    public string Category { get; set; } = null!;
}
