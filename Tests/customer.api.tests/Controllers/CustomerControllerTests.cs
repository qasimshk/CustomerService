namespace customer.api.tests.Controllers;

using System.Net;
using System.Text;
using System.Text.Json;
using core.Model;
using core.Model.Requests;
using core.Model.Responses;
using core.Model.Results;
using customer.api.tests.Fixtures;
using TestData;

public class CustomerControllerTests : IClassFixture<DockerContainerMsSqlFixture>
{
    // https://www.youtube.com/watch?v=xs8gNQjCXw0
    // https://www.youtube.com/watch?v=Pk2d-qm5KwE&t=603s

    private readonly HttpClient _httpClient;

    private readonly DockerContainerMsSqlFixture _factory;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CustomerControllerTests(DockerContainerMsSqlFixture factory)
    {
        _factory = factory;

        _httpClient = _factory.CreateClient();

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    // Naming Convension: [ThingUnderTest]_Should_[ExpectedResult]_[Conditions]
    // Example: Create_Should_ReturnNull_WhenValueIsNull
    [Fact]
    public async Task GetAllCustomers_Should_ReturnOkObjectResult()
    {
        // Act
        var response = await _httpClient.GetAsync("api/customer");
        var stringResult = await response.Content.ReadAsStringAsync();
        var serviceResponse = JsonSerializer.Deserialize<Result<List<CustomerDto>>>(stringResult, _jsonSerializerOptions)!;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        serviceResponse.Value.Should().NotBeNull();

        serviceResponse.IsSuccess.Should().BeTrue();

        serviceResponse.Value.Should().AllBeOfType<CustomerDto>();
    }

    [Fact]
    public async Task GetApiServices_Should_ReturnOkObjectResult_WhenDefaultPaginationParametersSent()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var pagination = "{\"TotalCount\":1425,\"PageSize\":10,\"CurrentPage\":1,\"TotalPages\":143,\"HasNext\":true,\"HasPrevious\":false}";

        // Act
        var response = await _httpClient.GetAsync($"/api/customer/search?PageNumber={pageNumber}&PageSize={pageSize}");
        var stringResult = await response.Content.ReadAsStringAsync();
        var serviceResponse = JsonSerializer.Deserialize<PagedList<WebsiteInfoDto>>(stringResult)!;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Headers.GetValues("X-Pagination").First().Should().Be(pagination);

        serviceResponse.Count.Should().Be(pageSize);
    }

    [Fact]
    public async Task GetApiServices_Should_ReturnOkObjectResult_WhenPaginationAndSearchParametersSent()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var pagination = "{\"TotalCount\":1,\"PageSize\":10,\"CurrentPage\":1,\"TotalPages\":1,\"HasNext\":false,\"HasPrevious\":false}";

        // Act
        var response = await _httpClient.GetAsync($"/api/customer/search?PageNumber={pageNumber}&PageSize={pageSize}&Category=Books&Api=A Bíblia Digital");
        var stringResult = await response.Content.ReadAsStringAsync();
        var serviceResponse = JsonSerializer.Deserialize<PagedList<WebsiteInfoDto>>(stringResult)!;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Headers.GetValues("X-Pagination").First().Should().Be(pagination);

        serviceResponse.Count.Should().Be(1);
    }

    [Theory]
    [InlineData("96d0baa3-55f0-4d09-8ffa-f32d27dc4870")]
    [InlineData("ee39b52b-25aa-432b-a694-f92ff593029a")]
    [InlineData("77eb8dcc-a3d9-4cbe-8342-0ffd4078f7e8")]
    [InlineData("ab719a26-7c52-49f5-babf-92c8f5308ba1")]
    [InlineData("60ec1279-d5b2-41c8-8488-4825d509f8c2")]
    public async Task GetCustomerByReferenceNumber_Should_ReturnOkObjectResult_WhenCorrectReferenceNumberSent(string referenceNumber)
    {
        // Act
        var response = await _httpClient.GetAsync($"/api/customer/{referenceNumber}");
        var stringResult = await response.Content.ReadAsStringAsync();
        var serviceResponse = JsonSerializer.Deserialize<Result<CustomerDto>>(stringResult, _jsonSerializerOptions)!;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        serviceResponse.IsSuccess.Should().BeTrue();

        serviceResponse.Value.RefrenceNumber.Should().Be(referenceNumber);

        serviceResponse.Value.Should().BeOfType<CustomerDto>();
    }

    [Fact]
    public async Task GetCustomerByReferenceNumber_Should_ReturnNotFound_WhenInCorrectReferenceNumberSent()
    {
        // Arrange
        var referenceNumber = "aba9358c-d46c-4d6e-a639-9fc09fdaf253";

        // Act
        var response = await _httpClient.GetAsync($"/api/customer/{referenceNumber}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestEntityValidation_WillValidateEntityTypeConfiguration_ReturnInternalServerError()
    {
        // Arrange
        var request = new CustomerCreateModel
        {
            Title = "123456",
            FirstName = "Abby",
            LastName = "Will",
            ContactNumber = 123456789,
            Dob = DateTimeOffset.Now,
            EmailAddress = $"{Guid.NewGuid()}@test.com",
            ReferenceNumber = Guid.NewGuid(),
            Address = new CustomerAddress
            {
                Street = "test street",
                City = "Test",
                Country = "Test",
                Nation = "England",
                Postcode = "SE10 9QR"
            }
        };

        var serviceRequest = GetRequestToStringConstant(request);

        // Act
        var response = await _httpClient.PostAsync($"/api/customer", serviceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Theory]
    [ClassData(typeof(CustomerCreateModelValidTestData))]
    public async Task CreateCustomer_Should_ReturnCreated_WhenValidRequestSent(CustomerCreateModel request)
    {
        // Arrange
        var serviceRequest = GetRequestToStringConstant(request);
        var locationUrl = $"http://localhost/api/customer/{request.ReferenceNumber}";

        // Act
        var response = await _httpClient.PostAsync($"/api/customer", serviceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Headers.GetValues("Location").First().Should().Be(locationUrl);
    }

    [Theory]
    [ClassData(typeof(CustomerCreateModelInValidTestData))]
    public async Task CreateCustomer_Should_ReturnBadRequest_WhenAnInValidRequestSent(CustomerCreateModel request)
    {
        // Arrange
        var serviceRequest = GetRequestToStringConstant(request);

        // Act
        var response = await _httpClient.PostAsync($"/api/customer", serviceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCustomer_Should_ReturnNoContent_WhenValidRequestSent()
    {
        // Arrange
        var serviceRequest = GetRequestToStringConstant(new CustomerCreateModel
        {
            Title = "Mr",
            FirstName = "Abby",
            LastName = "Will"
        });

        var referenceNumber = "96d0baa3-55f0-4d09-8ffa-f32d27dc4870";

        // Act
        var response = await _httpClient.PutAsync($"/api/customer/{referenceNumber}", serviceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateCustomer_Should_ReturnNoFound_WhenAnInValidReferenceNumberSent()
    {
        // Arrange
        var serviceRequest = GetRequestToStringConstant(new CustomerCreateModel
        {
            Title = "Mr",
            FirstName = "Abby",
            LastName = "Will"
        });

        var referenceNumber = "f5315449-94e6-4e56-81f6-afaf96c62d0b";

        // Act
        var response = await _httpClient.PutAsync($"/api/customer/{referenceNumber}", serviceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCustomer_Should_ReturnNoContent_WhenCorrectReferenceNumberSent()
    {
        // Arrange
        var referenceNumber = "96d0baa3-55f0-4d09-8ffa-f32d27dc4870";

        // Act
        var response = await _httpClient.DeleteAsync($"/api/customer/{referenceNumber}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCustomer_Should_ReturnNotFound_WhenAnInCorrectReferenceNumberSent()
    {
        // Arrange
        var referenceNumber = "f5315449-94e6-4e56-81f6-afaf96c62d0b";

        // Act
        var response = await _httpClient.DeleteAsync($"/api/customer/{referenceNumber}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static StringContent GetRequestToStringConstant(object request)
    {
        var customer = JsonSerializer.Serialize(request);

        return new StringContent(customer, Encoding.UTF8, "application/json");
    }
}
