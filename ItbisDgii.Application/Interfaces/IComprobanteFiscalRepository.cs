using ItbisDgii.Application.Wrappers;
using ItbisDgii.Domain.Entities;

namespace ItbisDgii.Application.Interfaces
{
    public interface IComprobanteFiscalRepository : IRepository<ComprobanteFiscal>
    {
        
        Task<int> CountAsync(CancellationToken cancellationToken);
        Task<IEnumerable<ComprobanteFiscal>> GetByRncCedulaAsync(string rncCedula, CancellationToken cancellationToken = default);
    }
}
