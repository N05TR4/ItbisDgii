using FluentValidation;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetTotalITBISByRncCedula
{
    public class GetTotalITBISByRncCedulaQueryValidator : AbstractValidator<GetTotalITBISByRncCedulaQuery>
    {
        public GetTotalITBISByRncCedulaQueryValidator()
        {
            RuleFor(q => q.RncCedula)
                .NotEmpty().WithMessage("RNC/Cédula es requerido")
                .Must(rncCedula => rncCedula.Length == 9 || rncCedula.Length == 11)
                .WithMessage("RNC/Cédula debe tener 9 dígitos para persona jurídica o 11 dígitos para persona física");
        }
    }
}
