namespace customer.core.Validations;

using data.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Model.Requests;

public class CustomerCreateValidator : AbstractValidator<CustomerCreateModel>
{
    private readonly CustomerDbContext _customerDbContext;

    public CustomerCreateValidator(CustomerDbContext customerDbContext)
    {
        _customerDbContext = customerDbContext;

        RuleFor(x => x.Title)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Length(2, 6);

        RuleFor(x => x.FirstName)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Length(2, 20);

        RuleFor(x => x.LastName)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Length(2, 20);

        RuleFor(x => x.Dob)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.ContactNumber)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.EmailAddress)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (emailAddress, cancellation) =>
                !await _customerDbContext.Customers.AnyAsync(x => x.EmailAddress.ToLower() == emailAddress.ToLower(), cancellationToken: cancellation))
            .WithMessage(x => $"Email address '{x.EmailAddress}' already in use");

        RuleFor(x => x.Address).ChildRules(customerAddress =>
        {
            customerAddress.RuleFor(x => x.Street)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Length(5, 30);

            customerAddress.RuleFor(x => x.City)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Length(2, 30);

            customerAddress.RuleFor(x => x.Postcode)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Length(6, 10);

            customerAddress.RuleFor(x => x.Nation)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MustAsync(async (nation, cancellation) =>
                await _customerDbContext.Nations.AnyAsync(x => x.Name.ToLower() == nation.ToLower(), cancellationToken: cancellation))
            .WithMessage(x => $"Invalid nation: {x.Nation}");

            customerAddress.RuleFor(x => x.Country)
            .Cascade(cascadeMode: CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Length(2, 6);
        });
    }
}
