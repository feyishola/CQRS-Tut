using System.Data;
using FluentValidation;

namespace OrdersAPI.Commands
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.TotalCost).GreaterThanOrEqualTo(0);
        }
    }
}