using System.Linq;
using System.Threading.Tasks;

namespace Dal.Base
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        void Add(TEntity entity);
        void Delete(TEntity entity);
        Task DeleteAsync(int id);
        Task<TEntity> GetByIdAsync(int id);
        IQueryable<TEntity> Query(bool noTracking = false);
        TEntity Insert(TEntity entity);
        void Update(TEntity entity);
    }
}