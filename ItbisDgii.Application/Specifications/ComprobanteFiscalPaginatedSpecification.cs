using ItbisDgii.Domain.Entities;

namespace ItbisDgii.Application.Specifications
{
    public class ComprobanteFiscalPaginatedSpecification : BaseSpecification<ComprobanteFiscal>
    {
        public ComprobanteFiscalPaginatedSpecification(int pageNumber, int pageSize)
            : base(x => true)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
            ApplyOrderBy(c => c.CreatedAt);
            AddInclude(c => c.Contribuyente);
        }
    }

}
