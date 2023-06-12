namespace customer.core.Abstractions.Services;

using Model.Requests;
using Model.Responses;
using Model.Results;

public interface ICustomerService
{
    Task<Result<List<CustomerDto>>> GetAllCustomers();

    Task<Result<CustomerDto>> GetCustomerByReferenceNumber(Guid referenceNumber);

    Task<Result> AddCustomer(CustomerCreateModel model);

    Task<Result> UpdateCustomer(CustomerUpdateModel model, Guid referenceNumber);

    Task<Result> DeleteCustomer(Guid referenceNumber);
}
