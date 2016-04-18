using System.Data.Entity;
using Dal.Base;

namespace Dal.Test.Mocks
{
    public class RepositoryFactoryMock:IRepositoryFactory
    {
        private readonly DbContext _dbContext;

        public RepositoryFactoryMock(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<T> CreateRepository<T>() where T : EntityBase
        {
            return new Repository<T>(_dbContext);
        }
    }
}