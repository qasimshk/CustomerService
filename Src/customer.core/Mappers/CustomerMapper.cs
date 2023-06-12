namespace customer.core.Mappers;

using Abstractions.Mappers;
using data.Entities;
using Model.Requests;
using Model.Responses;

public class CustomerMapper : ICustomerMapper
{
    public CustomerDto Map(Customer from) => new()
    {
        FullName = from.GetFullName(),
        Age = from.GetAge(),
        CompleteAddress = from.GetCompleteAddress(),
        ContactNumber = from.ContactNumber,
        DateOfBirth = from.GetDateOfBirth(),
        EmailAddress = from.EmailAddress,
        RefrenceNumber = from.ReferenceNumber
    };

    public Customer Map(CustomerCreateModel from) => new()
    {
        Title = from.Title,
        FirstName = from.FirstName,
        LastName = from.LastName,
        Dob = from.Dob,
        ContactNumber = from.ContactNumber,
        EmailAddress = from.EmailAddress,
        ReferenceNumber = from.ReferenceNumber,
        Address = new()
        {
            Street = from.Address.Street,
            City = from.Address.City,
            Postcode = from.Address.Postcode,
            Nation = from.Address.Nation,
            Country = from.Address.Country
        },
    };
}
