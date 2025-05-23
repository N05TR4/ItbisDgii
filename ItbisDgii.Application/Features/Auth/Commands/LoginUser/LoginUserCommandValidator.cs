using FluentValidation;

namespace ItbisDgii.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email es requerido");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password es requerido");
        }
    }
}
