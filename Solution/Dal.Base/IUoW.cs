using System.Threading.Tasks;

namespace Dal.Base
{
    public interface IUoW
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : EntityBase;
        Task SaveChangesAsync();
        void SaveChanges();
    }
}
