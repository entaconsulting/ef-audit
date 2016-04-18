namespace Dal.Base
{
    public interface IRepositoryFactory
    {
        IRepository<T> CreateRepository<T>() where T:EntityBase;
    }
}
