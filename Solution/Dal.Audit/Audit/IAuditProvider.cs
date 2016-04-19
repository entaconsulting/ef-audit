using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Audit.Audit
{
    public interface IAuditProvider
    {
        IEnumerable<AuditDataDetail> GetAuditTrail<T>(T entity, AuditConfigurationEntry config,
            params Expression<Func<T, object>>[] fields) where T : class;

        IEnumerable<AuditDataDetail> GetAuditTrail(string entityName, string entityKey, params string[] fields);

        void AddAudit(DbEntityEntry entry, DateTime date, string user, AuditConfigurationEntry config, EntityState writeMode);

        IEnumerable<AuditDataDetail> GetAuditTrailByCompositeKey(string entityName, string compositeKey,
            params string[] fields);

        IEnumerable<AuditDataDetailRelation> GetAuditTrailByCompositeKeyRelation(string entityName, string compositeKey);
        void Write();
        Task WriteAsync();
    }
}