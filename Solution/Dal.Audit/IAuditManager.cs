using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dal.Audit
{
    /// <summary>
    /// Interface IAuditManager
    /// </summary>
    public interface IAuditManager
    {
        /// <summary>
        /// Gets the last update.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidad">The entidad.</param>
        /// <returns>AuditData.</returns>
        AuditData GetLastUpdate<T>(T entidad) where T : class;
        /// <summary>
        /// Gets the last update.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidad">The entidad.</param>
        /// <param name="campos">The campos.</param>
        /// <returns>AuditData.</returns>
        AuditData GetLastUpdate<T>(T entidad, params Expression<Func<T, object>>[] campos) where T : class;
        /// <summary>
        /// Gets the last update.
        /// </summary>
        /// <param name="nombreEntidad">The nombre entidad.</param>
        /// <param name="claveEntidad">The clave entidad.</param>
        /// <returns>AuditData.</returns>
        AuditData GetLastUpdate(string nombreEntidad, string claveEntidad);
        /// <summary>
        /// Gets the last update by composite key.
        /// </summary>
        /// <param name="nombreEntidad">The nombre entidad.</param>
        /// <param name="copositeKey">The coposite key.</param>
        /// <returns>AuditData.</returns>
        AuditData GetLastUpdateByCompositeKey(string nombreEntidad, string copositeKey);

        /// <summary>
        /// Gets the field history by composite key relation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compositeKey">The composite key.</param>
        /// <returns>IEnumerable&lt;AuditDataDetailRelation&gt;.</returns>
        IEnumerable<AuditDataDetailRelation> GetFieldHistoryByCompositeKeyRelation<T>(string compositeKey)
            where T : class;

        /// <summary>
        /// Gets the field history by composite key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compositeKey">The composite key.</param>
        /// <param name="campos">The campos.</param>
        /// <returns>IEnumerable&lt;AuditDataDetail&gt;.</returns>
        IEnumerable<AuditDataDetail> GetFieldHistoryByCompositeKey<T>(string compositeKey,
            params Expression<Func<T, object>>[] campos) where T : class;

        /// <summary>
        /// Gets the field history.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidad">The entidad.</param>
        /// <param name="campos">The campos.</param>
        /// <returns>IEnumerable&lt;AuditDataDetail&gt;.</returns>
        IEnumerable<AuditDataDetail> GetFieldHistory<T>(T entidad, params Expression<Func<T, object>>[] campos)
            where T : class;

        /// <summary>
        /// Gets the field history.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="claveEntidad">The clave entidad.</param>
        /// <param name="campos">The campos.</param>
        /// <returns>IEnumerable&lt;AuditDataDetail&gt;.</returns>
        IEnumerable<AuditDataDetail> GetFieldHistory<T>(string claveEntidad, params Expression<Func<T, object>>[] campos)
            where T : class;


        /// <summary>
        /// Writes the audit added.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="date">The date.</param>
        void WriteAuditAdded(DbEntityEntry entry, DateTime date);
        /// <summary>
        /// Writes the audit modified.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="date">The date.</param>
        void WriteAuditModified(DbEntityEntry entry, DateTime date);
        /// <summary>
        /// Writes the audit deleted.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="date">The date.</param>
        void WriteAuditDeleted(DbEntityEntry entry, DateTime date);
        /// <summary>
        /// Commits this instance.
        /// </summary>
        void Commit();
        /// <summary>
        /// Commits the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task CommitAsync();

    }


}