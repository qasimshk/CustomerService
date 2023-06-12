namespace customer.api.Controllers;

using System.Net;
using System.Text.Json;
using Configurations;
using core.Abstractions.Services;
using core.Model.Requests;
using core.Model.Responses;
using core.Model.Results;
using customer.core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[Route("api/customer")]
public class CustomerController : ApiControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IPublicService _publicService;

    public CustomerController(IOptions<ApplicationConfig> baseApiConfigOptions,
        ICustomerService customerService,
        IPublicService publicService) : base(baseApiConfigOptions)
    {
        _customerService = customerService;
        _publicService = publicService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<List<CustomerDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllCustomers() => Ok(await _customerService.GetAllCustomers());

    [HttpGet("{referenceNumber}", Name = nameof(GetCustomerByReferenceNumber))]
    [ProducesResponseType(typeof(Result<CustomerDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetCustomerByReferenceNumber(Guid referenceNumber)
    {
        var result = await _customerService.GetCustomerByReferenceNumber(referenceNumber);

        return result.IsSuccess ? Ok(result) : NotFound(result.ErrorMessage);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedList<WebsiteInfoDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetApiServices([FromQuery] ServiceParametersModel serviceParameters)
    {
        var response = await _publicService.GetWebsiteDetails(serviceParameters);

        var metadata = new
        {
            response.TotalCount,
            response.PageSize,
            response.CurrentPage,
            response.TotalPages,
            response.HasNext,
            response.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateModel model)
    {
        var result = await _customerService.AddCustomer(model);

        return result.IsSuccess ?
            CreatedAtRoute(nameof(GetCustomerByReferenceNumber), new { referenceNumber = model.ReferenceNumber }, result) :
            BadRequest(result.ErrorMessages);
    }

    [HttpPut("{referenceNumber}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateCustomer([FromRoute] Guid referenceNumber, [FromBody] CustomerUpdateModel model)
    {
        var result = await _customerService.UpdateCustomer(model, referenceNumber);

        return result.StatusCode switch
        {
            HttpStatusCode.BadRequest => BadRequest(result.ErrorMessages),
            HttpStatusCode.NotFound => NotFound(),
            _ => NoContent()
        };
    }

    [HttpDelete("{referenceNumber}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteCustomer([FromRoute] Guid referenceNumber)
    {
        var result = await _customerService.DeleteCustomer(referenceNumber);

        return result.IsSuccess ? NoContent() : NotFound(result.ErrorMessages);
    }
}
