using AutenticacaoJWT.Application.DTO;
using FluentValidation;

namespace AutenticacaoJWT.Application.Validators
{
    public class CreateUserDTOValidator : AbstractValidator<CreateUserDTO>
    {
        public CreateUserDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("A senha deve conter ao menos uma letra maiúscula.")
                .Matches("[a-z]").WithMessage("A senha deve conter ao menos uma letra minúscula.")
                .Matches("[0-9]").WithMessage("A senha deve conter ao menos um número.");
        }
    }
}
