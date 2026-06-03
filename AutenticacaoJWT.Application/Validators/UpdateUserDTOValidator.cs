using AutenticacaoJWT.Application.DTO;
using FluentValidation;

namespace AutenticacaoJWT.Application.Validators
{
    public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
    {
        public UpdateUserDTOValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
            When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
            {
                RuleFor(x => x.Password!)
                    .MinimumLength(8)
                    .Matches("[A-Z]").WithMessage("A senha deve conter ao menos uma letra maiúscula.")
                    .Matches("[a-z]").WithMessage("A senha deve conter ao menos uma letra minúscula.")
                    .Matches("[0-9]").WithMessage("A senha deve conter ao menos um número.");
            });
        }
    }
}
