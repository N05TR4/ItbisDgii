using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Specifications;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Infraestructure.Persistence.Context;

namespace ItbisDgii.Infraestructure.Persistence.Repositories
{
    public class ContribuyenteRepository : BaseRepository<Contribuyente>, IContribuyenteRepository
    {
        public ContribuyenteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Contribuyente?> GetByRncCedulaAsync(string rncCedula, CancellationToken cancellationToken = default)
        {
            var spec = new ContribuyenteWithComprobantesFiscalesSpecification(rncCedula);
            var contribuyentes = await GetAsync(spec, cancellationToken);
            return contribuyentes.FirstOrDefault();
        }

        public async Task<IEnumerable<Contribuyente>> GetAllWithComprobantesAsync(CancellationToken cancellationToken = default)
        {
            var spec = new ContribuyenteWithComprobantesFiscalesSpecification();
            return await GetAsync(spec, cancellationToken);
        }
    }
}
