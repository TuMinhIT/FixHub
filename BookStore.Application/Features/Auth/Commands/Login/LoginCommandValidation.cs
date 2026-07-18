using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidation: AbstractValidator<LoginCommand>
    {
        public LoginCommandValidation()
        {
            RuleFor(x=> x.Email).NotEmpty().EmailAddress();
            RuleFor(x=> x.Password).NotEmpty().MinimumLength(6);
        }

    }
}
