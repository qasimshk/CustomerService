namespace customer.core.Abstractions.Mappers;

using data.Entities;
using Model.Requests;
using Model.Responses;

public interface ICustomerMapper :
    IMapper<Customer, CustomerDto>,
    IMapper<CustomerCreateModel, Customer>
{ }
