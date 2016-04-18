using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Dal.Base
{
    public class UoW : IUoW, IDisposable
    {
        private bool _disposed;
        private readonly DbContext _context;
        private readonly IRepositoryFactory _repositoryFactory;

        public UoW(DbContext context, IRepositoryFactory repositoryFactory )
        {
            _context = context;
            _repositoryFactory = repositoryFactory;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : EntityBase
        {
            return _repositoryFactory.CreateRepository<TEntity>();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

    }

}
