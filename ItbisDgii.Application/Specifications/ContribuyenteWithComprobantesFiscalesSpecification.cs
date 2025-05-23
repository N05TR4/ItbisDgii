using ItbisDgii.Domain.Entities;

namespace ItbisDgii.Application.Specifications
{
    public class ContribuyenteWithComprobantesFiscalesSpecification : BaseSpecification<Contribuyente>
    {
        public ContribuyenteWithComprobantesFiscalesSpecification(string rncCedula)
            : base(c => c.RncCedula == rncCedula)
        {
            AddInclude(c => c.ComprobantesFiscales);
        }

        public ContribuyenteWithComprobantesFiscalesSpecification()
            : base(null)
        {
            AddInclude(c => c.ComprobantesFiscales);
        }
    }
}
