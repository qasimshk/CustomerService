namespace customer.api.tests.Entities;

using System.Net;
using System.Text;
using System.Text.Json;
using customer.api.tests.Fixtures;
using customer.core.Model.Requests;
using Microsoft.AspNetCore.Mvc.Testing;

public class CustomerEntityTests
{
    private readonly HttpClient _httpClient;

    public CustomerEntityTests()
    {
        var webAppFactory = new WebApplicationFactory<Program>();

        _httpClient = webAppFactory.CreateDefaultClient();
    }

    [Fact]
    public void Test_BogusNuGet_ReturnFiveRecords()
    {
        // Act
        var data = DataFixture.GetCustomerFaker(5);

        // Assert
        data.Count.Should().Be(5);
    }

    [Fact]
    public async Task TestToProve_InMemoryWillNotValidateEntityTypeConfiguration_ReturnCreate()
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

        var customer = JsonSerializer.Serialize(request);

        var serviceRequest = new StringContent(customer, Encoding.UTF8, "application/json");

        var locationUrl = $"http://localhost/api/customer/{request.ReferenceNumber}";

        // Act
        var response = await _httpClient.PostAsync($"/api/customer", serviceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Headers.GetValues("Location").First().Should().Be(locationUrl);
    }
}
