using System.Data.Entity;
using Audit;

namespace Dal.Audit
{
    public static class AuditExtension
    {

        public static T BeginAudit<T>(this T dbContext, IAuditManager auditManager) where T :DbContext
        {
            return (T)new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy(typeof(T), new AuditInterceptor(auditManager));
        }
    }
}


