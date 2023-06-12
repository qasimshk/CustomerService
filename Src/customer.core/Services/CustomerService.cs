namespace customer.core.Services;

using System.Net;
using Abstractions.Mappers;
using Abstractions.Services;
using data.Context;
using data.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Model.Requests;
using Model.Responses;
using Model.Results;

public class CustomerService : ICustomerService
{
    private readonly CustomerDbContext _customerDbContext;
    private readonly ICustomerMapper _customerMapper;
    private readonly IValidator<CustomerCreateModel> _validatorCreate;
    private readonly IValidator<CustomerUpdateModel> _validatorUpdate;

    public CustomerService(CustomerDbContext customerDbContext,
        ICustomerMapper customerMapper,
        IValidator<CustomerCreateModel> validatorCreate,
        IValidator<CustomerUpdateModel> validatorUpdate)
    {
        _customerDbContext = customerDbContext;
        _customerMapper = customerMapper;
        _validatorCreate = validatorCreate;
        _validatorUpdate = validatorUpdate;
    }

    public async Task<Result<List<CustomerDto>>> GetAllCustomers()
    {
        var response = GetCustomersQueryable().Select(customer => _customerMapper.Map(customer));

        return Result<List<CustomerDto>>.SuccessResult(await response.ToListAsync());
    }

    public async Task<Result<CustomerDto>> GetCustomerByReferenceNumber(Guid referenceNumber)
    {
        var customer = await GetCustomersQueryable()
            .FirstOrDefaultAsync(x => x.ReferenceNumber == referenceNumber);

        if (customer is null)
        {
            return Result<CustomerDto>.FailedResult(string.Empty, HttpStatusCode.NotFound);
        }

        var result = _customerMapper.Map(customer);

        return Result<CustomerDto>.SuccessResult(result);
    }

    public async Task<Result> AddCustomer(CustomerCreateModel model)
    {
        var result = await _validatorCreate.ValidateAsync(model);

        if (result.IsValid)
        {
            var entity = _customerMapper.Map(model);

            await _customerDbContext.AddAsync(entity);

            await _customerDbContext.SaveChangesAsync();

            return Result.SuccessResult();
        }

        return Result.FailedResult(result.Errors.Select(x => x.ErrorMessage).ToList(),
            HttpStatusCode.BadRequest);
    }

    public async Task<Result> UpdateCustomer(CustomerUpdateModel model, Guid referenceNumber)
    {
        var result = await _validatorUpdate.ValidateAsync(model);

        if (result.IsValid)
        {
            var existingCustomer = await GetCustomersQueryable()
                .FirstOrDefaultAsync(x => x.ReferenceNumber == referenceNumber);

            if (existingCustomer is null)
            {
                return Result.FailedResult(null, HttpStatusCode.NotFound);
            }

            if (!string.IsNullOrEmpty(model.Title) && existingCustomer!.Title != model.Title)
            {
                existingCustomer!.Title = model.Title;
            }

            if (!string.IsNullOrEmpty(model.FirstName) && existingCustomer!.FirstName != model.FirstName)
            {
                existingCustomer!.FirstName = model.FirstName;
            }

            if (!string.IsNullOrEmpty(model.LastName) && existingCustomer!.LastName != model.LastName)
            {
                existingCustomer!.LastName = model.LastName;
            }

            if (model.Dob != DateTime.MinValue)
            {
                existingCustomer!.Dob = model.Dob;
            }

            if (model.ContactNumber > 0)
            {
                existingCustomer!.ContactNumber = model.ContactNumber;
            }

            _customerDbContext.Update(existingCustomer);

            await _customerDbContext.SaveChangesAsync();

            return Result.SuccessResult();
        }

        return Result.FailedResult(result.Errors.Select(x => x.ErrorMessage).ToList(),
            HttpStatusCode.BadRequest);
    }

    public async Task<Result> DeleteCustomer(Guid referenceNumber)
    {
        var existingCustomer = await GetCustomersQueryable()
                .FirstOrDefaultAsync(x => x.ReferenceNumber == referenceNumber);

        if (existingCustomer is null)
        {
            return Result.FailedResult(null, HttpStatusCode.NotFound);
        }

        _customerDbContext.Remove(existingCustomer);

        await _customerDbContext.SaveChangesAsync();

        return Result.SuccessResult();
    }

    private IQueryable<Customer> GetCustomersQueryable() => _customerDbContext.Customers
            .Include(x => x.Address)
            .AsQueryable();
}
