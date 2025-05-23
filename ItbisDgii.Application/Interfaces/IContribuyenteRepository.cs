using ItbisDgii.Application.Wrappers;
using ItbisDgii.Domain.Entities;

namespace ItbisDgii.Application.Interfaces
{
    public interface IContribuyenteRepository : IRepository<Contribuyente>
    {
        Task<Contribuyente?> GetByRncCedulaAsync(string rncCedula, CancellationToken cancellationToken = default);
        Task<IEnumerable<Contribuyente>> GetAllWithComprobantesAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken);
    }
}
