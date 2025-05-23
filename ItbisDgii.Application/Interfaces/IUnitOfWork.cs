namespace ItbisDgii.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IContribuyenteRepository ContribuyenteRepository { get; }
        IComprobanteFiscalRepository ComprobanteFiscalRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
