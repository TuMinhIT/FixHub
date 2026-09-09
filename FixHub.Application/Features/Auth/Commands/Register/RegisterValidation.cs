

using FluentValidation;

namespace FixHub.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandValidator
     : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();


            RuleFor(x => x.Password)
                .MinimumLength(6);
            RuleFor(x => x.PhoneNumber).Length(10);
        }
    }
}
