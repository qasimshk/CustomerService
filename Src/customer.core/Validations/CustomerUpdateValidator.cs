namespace customer.core.Validations;

using FluentValidation;
using Model.Requests;

public class CustomerUpdateValidator : AbstractValidator<CustomerUpdateModel>
{
    public CustomerUpdateValidator()
    {
        RuleFor(x => x.Title)
            .Length(2, 6);

        RuleFor(x => x.FirstName)
            .Length(2, 20);

        RuleFor(x => x.LastName)
            .Length(2, 20);
    }
}
