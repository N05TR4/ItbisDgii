using FluentValidation;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Commands.CreateComprobanteFiscal
{
    public class CreateComprobanteFiscalCommandValidator : AbstractValidator<CreateComprobanteFiscalCommand>
    {
        public CreateComprobanteFiscalCommandValidator()
        {
            RuleFor(c => c.RncCedula)
                .NotEmpty().WithMessage("RNC/Cédula es requerido")
                .Must(rncCedula => rncCedula.Length == 9 || rncCedula.Length == 11)
                .WithMessage("RNC/Cédula debe tener 9 dígitos para persona jurídica o 11 dígitos para persona física");

            RuleFor(c => c.NCF)
                .NotEmpty().WithMessage("NCF es requerido")
                .MaximumLength(19).WithMessage("NCF no puede exceder 19 caracteres")
                .Matches(@"^[a-zA-Z][0-9]{2}[0-9]{8}$").WithMessage("NCF debe tener el formato correcto (ejemplo: E310000000001)");

            RuleFor(c => c.Monto)
                .GreaterThan(0).WithMessage("Monto debe ser mayor que cero");
        }
    }
}
