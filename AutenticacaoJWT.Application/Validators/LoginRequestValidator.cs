using AutenticacaoJWT.Application.DTO;
using FluentValidation;

namespace AutenticacaoJWT.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
