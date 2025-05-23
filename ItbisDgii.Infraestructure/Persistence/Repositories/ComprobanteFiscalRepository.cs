using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Specifications;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Infraestructure.Persistence.Context;

namespace ItbisDgii.Infraestructure.Persistence.Repositories
{
    public class ComprobanteFiscalRepository : BaseRepository<ComprobanteFiscal>, IComprobanteFiscalRepository
    {
        public ComprobanteFiscalRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ComprobanteFiscal>> GetByRncCedulaAsync(string rncCedula, CancellationToken cancellationToken = default)
        {
            var spec = new ComprobantesFiscalesByRncCedulaSpecification(rncCedula);
            return await GetAsync(spec, cancellationToken);
        }
    }
}
