using System.Data.Entity;

namespace Audit.Audit
{
    public static class AuditExtension
    {

        public static void BeginAudit(this DbContext dbContext, IAuditProvider auditProvider, AuditProfile auditProfile, IAppContext appContext)
        {
            //return new AuditManager(auditProvider, auditProfile, appContext, dbContext);

        }
    }
}


