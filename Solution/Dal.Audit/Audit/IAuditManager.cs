using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Audit.Audit
{
    public interface IAuditManager
    {
        AuditData GetLastUpdate<T>(T entidad) where T : class;
        AuditData GetLastUpdate<T>(T entidad, params Expression<Func<T, object>>[] campos) where T : class;
        AuditData GetLastUpdate(string nombreEntidad, string claveEntidad);
        AuditData GetLastUpdateByCompositeKey(string nombreEntidad, string copositeKey);

        IEnumerable<AuditDataDetailRelation> GetFieldHistoryByCompositeKeyRelation<T>(string compositeKey)
            where T : class;

        IEnumerable<AuditDataDetail> GetFieldHistoryByCompositeKey<T>(string compositeKey,
            params Expression<Func<T, object>>[] campos) where T : class;

        IEnumerable<AuditDataDetail> GetFieldHistory<T>(T entidad, params Expression<Func<T, object>>[] campos)
            where T : class;

        IEnumerable<AuditDataDetail> GetFieldHistory<T>(string claveEntidad, params Expression<Func<T, object>>[] campos)
            where T : class;


        void WriteAuditAdded(DbEntityEntry entry, DateTime date);
        void WriteAuditModified(DbEntityEntry entry, DateTime date);
        void WriteAuditDeleted(DbEntityEntry entry, DateTime date);
        void Commit();
        Task CommitAsync();

    }


}