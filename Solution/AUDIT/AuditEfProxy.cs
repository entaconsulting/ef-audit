using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Audit.Audit;
using Castle.DynamicProxy;

namespace Audit
{

    public class AuditInterceptor : IInterceptor
    {
        private readonly IAuditProvider _auditProvider;
        private readonly AuditProfile _auditProfile;
        private readonly IAppContext _appContext;

        public AuditInterceptor(IAuditProvider auditProvider, AuditProfile auditProfile, IAppContext appContext)
        {
            _auditProvider = auditProvider;
            _auditProfile = auditProfile;
            _appContext = appContext;

        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name == "SaveChanges")
            {
                SaveChanges(invocation);
            }
            else if (invocation.Method.Name == "SaveChangesAsync")
            {
                SaveChangesAsync(invocation);
                //invocation.Proceed();


            }
            else
            {
                invocation.Proceed();
            }



        }

        private void SaveChanges(IInvocation invocation)
        {
            var dbContext = (DbContext)invocation.InvocationTarget;

            dbContext.ChangeTracker.DetectChanges();

            var entries = dbContext.ChangeTracker.Entries().ToList();

            var date = DateTime.Now;

            //habilito la detección de cambios para las auditorias
            var autoDetectChanges = dbContext.Configuration.AutoDetectChangesEnabled;
            dbContext.Configuration.AutoDetectChangesEnabled = true;

            var added = entries.Where(a => a.State == EntityState.Added).ToList();
            //necesito materializar la lista antes de que cambie el estado de las entidades

            Audit(entries.Where(a => a.State == EntityState.Deleted).ToList(), date, EntityState.Deleted);
            Audit(entries.Where(a => a.State == EntityState.Modified).ToList(), date, EntityState.Modified);

            //las altas se loguean después del save para tener los ids
            invocation.Proceed();

            //ejecuto el audit de los added despues del save changes para obtener los ids generados
            Audit(added, date, EntityState.Added);

            _auditProvider.Write();

            //vuelvo a desactivar el autotedect
            dbContext.Configuration.AutoDetectChangesEnabled = autoDetectChanges;
        }

        private void SaveChangesAsync(IInvocation invocation)
        {
            var dbContext = (DbContext)invocation.InvocationTarget;

            dbContext.ChangeTracker.DetectChanges();

            var entries = dbContext.ChangeTracker.Entries().ToList();

            var date = DateTime.Now;

            //habilito la detección de cambios para las auditorias
            var autoDetectChanges = dbContext.Configuration.AutoDetectChangesEnabled;
            dbContext.Configuration.AutoDetectChangesEnabled = true;

            var added = entries.Where(a => a.State == EntityState.Added).ToList();
            //necesito materializar la lista antes de que cambie el estado de las entidades

            Audit(entries.Where(a => a.State == EntityState.Deleted).ToList(), date, EntityState.Deleted);
            Audit(entries.Where(a => a.State == EntityState.Modified).ToList(), date, EntityState.Modified);

            //las altas se loguean después del save para tener los ids

            invocation.Proceed();


            //((Task)invocation.ReturnValue)
           //.ContinueWith(task =>
           //{
               Audit(added, date, EntityState.Added);

               _auditProvider.Write();

               //vuelvo a desactivar el autotedect
               dbContext.Configuration.AutoDetectChangesEnabled = autoDetectChanges;
           //});

          


           



        }

        private void Audit(List<DbEntityEntry> entries, DateTime date, EntityState state)
        {

            switch (state)
            {
                case EntityState.Added:
                    entries.ForEach(e => WriteAuditAdded(e, date));
                    break;
                case EntityState.Modified:
                    entries.ForEach(e => WriteAuditModified(e, date));
                    break;
                case EntityState.Deleted:
                    entries.ForEach(e => WriteAuditDeleted(e, date));
                    break;
            }
        }

        #region configuracion



        #endregion


        #region write

        public void WriteAuditModified(DbEntityEntry entry, DateTime date)
        {
            //obtener la configuración de auditoría de la entidad
            var config = _auditProfile.GetConfiguration(entry.Entity.GetType());
            if (!config.IsAuditable) return;

            _auditProvider.AddAudit(entry, date, _appContext.UserName, config, EntityState.Modified);
        }

        public void WriteAuditDeleted(DbEntityEntry entry, DateTime date)
        {
            //obtener la configuración de auditoría de la entidad
            var config = _auditProfile.GetConfiguration(entry.Entity.GetType());
            if (!config.IsAuditable) return;

            _auditProvider.AddAudit(entry, date, _appContext.UserName, config, EntityState.Deleted);
        }

        public void WriteAuditAdded(DbEntityEntry entry, DateTime date)
        {
            //obtener la configuración de auditoría de la entidad
            var config = _auditProfile.GetConfiguration(entry.Entity.GetType());
            if (!config.IsAuditable) return;

            _auditProvider.AddAudit(entry, date, _appContext.UserName, config, EntityState.Added);
        }

        #endregion

        #region queries

        public AuditData GetLastUpdate<T>(T entidad) where T : class
        {
            return GetLastUpdate<T>(entidad, new Expression<Func<T, object>>[] { });
        }

        public AuditData GetLastUpdate<T>(T entidad, params Expression<Func<T, object>>[] campos) where T : class
        {
            var auditTrail = _auditProvider.GetAuditTrail(entidad, _auditProfile.GetConfiguration(typeof(T)), campos);

            var lastUpdate = auditTrail.OrderByDescending(d => d.ChangeDate).FirstOrDefault();
            return lastUpdate;
        }

        public AuditData GetLastUpdate(string nombreEntidad, string claveEntidad)
        {
            var auditTrail = _auditProvider.GetAuditTrail(nombreEntidad, claveEntidad, new string[] { });
            var lastUpdate = auditTrail.OrderByDescending(d => d.ChangeDate).FirstOrDefault();
            return lastUpdate;

        }

        public AuditData GetLastUpdateByCompositeKey(string nombreEntidad, string copositeKey)
        {
            var auditTrail = _auditProvider.GetAuditTrailByCompositeKey(nombreEntidad, copositeKey, new string[] { });
            var lastUpdate = auditTrail.OrderByDescending(d => d.ChangeDate).FirstOrDefault();
            return lastUpdate;

        }

        public IEnumerable<AuditDataDetail> GetFieldHistory<T>(T entidad, params Expression<Func<T, object>>[] campos)
            where T : class
        {
            var auditTrail = _auditProvider.GetAuditTrail(entidad, _auditProfile.GetConfiguration(typeof(T)), campos);
            return auditTrail;
        }

        public IEnumerable<AuditDataDetail> GetFieldHistory<T>(string claveEntidad,
            params Expression<Func<T, object>>[] campos) where T : class
        {

            return _auditProvider.GetAuditTrail(typeof(T).Name, claveEntidad,
                campos.Select(Helpers.GetFullPropertyName).ToArray());
        }

        public IEnumerable<AuditDataDetail> GetFieldHistoryByCompositeKey<T>(string compositeKey,
            params Expression<Func<T, object>>[] campos) where T : class
        {

            return _auditProvider.GetAuditTrailByCompositeKey(typeof(T).Name, compositeKey,
                campos.Select(Helpers.GetFullPropertyName).ToArray());
        }

        public IEnumerable<AuditDataDetailRelation> GetFieldHistoryByCompositeKeyRelation<T>(string compositeKey)
            where T : class
        {

            return _auditProvider.GetAuditTrailByCompositeKeyRelation(typeof(T).Name, compositeKey).ToArray();
        }


        #endregion
    }

    public class AuditConfiguration
    {
        public bool Generic { get; set; }
        public LambdaExpression EntityKey { get; set; }
        public String EntityKeyName { get; set; }
        public Func<object, string> CompositeKeyFunc { set; get; }
        public IList<AuditFieldDefinition> AuditFields { get; set; }
        public IList<AuditConfigurationEntry.AuditConfigurationReferenceEntry> AuditReferences { get; set; }
        public bool IgnoreIfNoFieldChanged { get; set; }
    }
}
