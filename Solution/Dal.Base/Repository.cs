using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dal.Base
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
      /// <summary>
        /// Contexto de acceso a la base de datos.
        /// </summary>
        protected readonly DbContext Context;

        /// <summary>
        /// DbSet correspondiente al tipo de entidad que maneja éste repositorio.
        /// </summary>
        protected readonly DbSet<TEntity> DbSet;


        public Repository(DbContext dbContext)
        {
            this.Context = dbContext;
            this.DbSet = this.Context.Set<TEntity>();
        }


        public virtual IQueryable<TEntity> Query(bool noTracking = false)
        {
            return noTracking ? Context.Set<TEntity>().AsNoTracking() : Context.Set<TEntity>();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await this.DbSet.AsNoTracking().SingleAsync(a => a.Id == id);
        }

        public void Add(TEntity entity)
        {
            this.DbSet.Add(entity);
        }

        public TEntity Insert(TEntity entity)
        {
            
            //Primero se atacha el grafo, para que accidentalmente no entren como INSERT entidades asociadas a la que se está insertando
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            //Ahora se marca como ADDED la entidad
            Context.Entry(entity).State = EntityState.Added;
            return entity;
        }

        public virtual void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.Entry(entity).Property(p => p.RowVersion).OriginalValue = entity.RowVersion;
        }

        public async Task DeleteAsync(int id)
        {
            TEntity entity = await DbSet.SingleAsync(a => a.Id == id);
            if (entity != null)
                this.Delete(entity);
        }

        /// <summary>
        /// Elimina de la base de datos una istancia de entidad.
        /// </summary>
        /// <param name="entity">Instancia de la entidad a borrar.</param>
        public virtual void Delete(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }

    }
}
