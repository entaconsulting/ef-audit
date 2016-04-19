using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Dal.Audit;

namespace Audit
{

    public class AuditInterceptor : IInterceptor
    {
        private readonly IAuditManager _auditManager;

        public AuditInterceptor(IAuditManager auditManager)
        {
            _auditManager = auditManager;

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
            var dbContext = invocation.InvocationTarget as DbContext;
            if (dbContext == null)
            {
                invocation.Proceed();
                return;
            }


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

            _auditManager.Commit();

            //vuelvo a desactivar el autotedect
            dbContext.Configuration.AutoDetectChangesEnabled = autoDetectChanges;
        }

        private void SaveChangesAsync(IInvocation invocation)
        {
            //si no tiene parámetros continuo sin hacer nada porque el que realmente ejecuta el savechanges es el overload con 1 parámetro
            if (invocation.Arguments.Length == 0)
            {
                invocation.Proceed();
                return;
            }

            var dbContext = invocation.InvocationTarget as DbContext;
            if (dbContext == null)
            {
                invocation.Proceed();
                return;
            }

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
            
            var internalTask = ((Task<int>) invocation.ReturnValue)
                .ContinueWith(task =>
                {
                    Audit(added, date, EntityState.Added);

                    _auditManager.Commit();

                    //vuelvo a desactivar el autotedect
                    dbContext.Configuration.AutoDetectChangesEnabled = autoDetectChanges;

                    return task.Result;
                });

            invocation.ReturnValue = internalTask;

        }

        private void Audit(List<DbEntityEntry> entries, DateTime date, EntityState state)
        {

            switch (state)
            {
                case EntityState.Added:
                    entries.ForEach(e => _auditManager.WriteAuditAdded(e, date));
                    break;
                case EntityState.Modified:
                    entries.ForEach(e => _auditManager.WriteAuditModified(e, date));
                    break;
                case EntityState.Deleted:
                    entries.ForEach(e => _auditManager.WriteAuditDeleted(e, date));
                    break;
            }
        }

    }
}
