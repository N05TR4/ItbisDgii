using FluentValidation;

namespace ItbisDgii.Application.Features.Contribuyentes.Commands.CreateContribuyente
{
    public class CreateContribuyenteCommandValidator : AbstractValidator<CreateContribuyenteCommand>
    {
        public CreateContribuyenteCommandValidator()
        {
            RuleFor(c => c.RncCedula)
                .NotEmpty().WithMessage("RNC/Cédula es requerido")
                .Must((command, rncCedula) =>
                {
                    if (string.IsNullOrWhiteSpace(rncCedula) || string.IsNullOrWhiteSpace(command.Tipo))
                        return false;

                    bool isPersonaFisica = command.Tipo.Equals("PersonaFisica", StringComparison.OrdinalIgnoreCase);
                    bool isPersonaJuridica = command.Tipo.Equals("PersonaJuridica", StringComparison.OrdinalIgnoreCase);

                    if (isPersonaFisica && rncCedula.Length != 11)
                        return false;

                    if (isPersonaJuridica && rncCedula.Length != 9)
                        return false;

                    return isPersonaFisica || isPersonaJuridica;
                })
                .WithMessage("RNC/Cédula debe tener 9 dígitos para persona jurídica o 11 dígitos para persona física");

            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("Nombre es requerido")
                .MaximumLength(100).WithMessage("Nombre no puede exceder 100 caracteres");

            RuleFor(c => c.Tipo)
                .NotEmpty().WithMessage("Tipo es requerido")
                .Must(tipo => !string.IsNullOrWhiteSpace(tipo) &&
                             (tipo.Equals("PersonaFisica", StringComparison.OrdinalIgnoreCase) ||
                              tipo.Equals("PersonaJuridica", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("Tipo debe ser 'PersonaFisica' o 'PersonaJuridica'");

            RuleFor(c => c.Estatus)
                .NotEmpty().WithMessage("Estatus es requerido")
                .Must(estatus => !string.IsNullOrWhiteSpace(estatus) &&
                                (estatus.Equals("Activo", StringComparison.OrdinalIgnoreCase) ||
                                 estatus.Equals("Inactivo", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("Estatus debe ser 'Activo' o 'Inactivo'");
        }
    }
}
