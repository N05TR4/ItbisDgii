using ItbisDgii.Domain.Entities;

namespace ItbisDgii.Application.Specifications
{
    public class ContribuyentesPaginatedSpecification : BaseSpecification<Contribuyente>
    {
        public ContribuyentesPaginatedSpecification(int pageNumber, int pageSize)
            : base(null)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
            ApplyOrderBy(c => c.Nombre);
            AddInclude(c => c.ComprobantesFiscales);
        }
    }
}
