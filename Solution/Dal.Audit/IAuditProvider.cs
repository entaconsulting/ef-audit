using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dal.Audit
{
    /// <summary>
    /// Interface IAuditProvider
    /// </summary>
    public interface IAuditProvider
    {
        /// <summary>
        /// Gets the audit trail.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>IEnumerable&lt;AuditDataDetail&gt;.</returns>
        IEnumerable<AuditDataDetail> GetAuditTrail<T>(T entity, AuditConfigurationEntry config,
            params Expression<Func<T, object>>[] fields) where T : class;

        /// <summary>
        /// Gets the audit trail.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="entityKey">The entity key.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>IEnumerable&lt;AuditDataDetail&gt;.</returns>
        IEnumerable<AuditDataDetail> GetAuditTrail(string entityName, string entityKey, params string[] fields);

        /// <summary>
        /// Adds the audit.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="date">The date.</param>
        /// <param name="user">The user.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="writeMode">The write mode.</param>
        void AddAudit(DbEntityEntry entry, DateTime date, string user, AuditConfigurationEntry config, EntityState writeMode);

        /// <summary>
        /// Gets the audit trail by composite key.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="compositeKey">The composite key.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>IEnumerable&lt;AuditDataDetail&gt;.</returns>
        IEnumerable<AuditDataDetail> GetAuditTrailByCompositeKey(string entityName, string compositeKey,
            params string[] fields);

        /// <summary>
        /// Gets the audit trail by composite key relation.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="compositeKey">The composite key.</param>
        /// <returns>IEnumerable&lt;AuditDataDetailRelation&gt;.</returns>
        IEnumerable<AuditDataDetailRelation> GetAuditTrailByCompositeKeyRelation(string entityName, string compositeKey);
        /// <summary>
        /// Writes this instance.
        /// </summary>
        void Write();
        /// <summary>
        /// Writes the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task WriteAsync();
    }
}