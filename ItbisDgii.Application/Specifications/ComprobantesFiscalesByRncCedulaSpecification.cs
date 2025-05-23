using ItbisDgii.Domain.Entities;

namespace ItbisDgii.Application.Specifications
{
    public class ComprobantesFiscalesByRncCedulaSpecification : BaseSpecification<ComprobanteFiscal>
    {
        public ComprobantesFiscalesByRncCedulaSpecification(string rncCedula)
            : base(c => c.RncCedula == rncCedula)
        {
        }
    }
}
