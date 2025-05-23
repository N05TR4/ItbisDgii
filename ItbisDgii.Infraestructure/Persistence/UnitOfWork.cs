using ItbisDgii.Application.Interfaces;
using ItbisDgii.Infraestructure.Persistence.Context;
using ItbisDgii.Infraestructure.Persistence.Repositories;

namespace ItbisDgii.Infraestructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IContribuyenteRepository _contribuyenteRepository;
        private IComprobanteFiscalRepository _comprobanteFiscalRepository;
        private bool _disposed;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IContribuyenteRepository ContribuyenteRepository =>
            _contribuyenteRepository ??= new ContribuyenteRepository(_dbContext);

        public IComprobanteFiscalRepository ComprobanteFiscalRepository =>
            _comprobanteFiscalRepository ??= new ComprobanteFiscalRepository(_dbContext);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;
        }
    }
}
