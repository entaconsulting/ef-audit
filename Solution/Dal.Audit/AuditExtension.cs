using System.Data.Entity;

namespace Dal.Audit
{
    /// <summary>
    /// Class AuditExtension.
    /// </summary>
    public static class AuditExtension
    {

        /// <summary>
        /// Begins the audit.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext">The database context.</param>
        /// <param name="auditManager">The audit manager.</param>
        /// <returns>T.</returns>
        public static T BeginAudit<T>(this T dbContext, IAuditManager auditManager) where T :DbContext
        {
            return (T)new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy(typeof(T), new AuditInterceptor(auditManager));
        }
    }
}


