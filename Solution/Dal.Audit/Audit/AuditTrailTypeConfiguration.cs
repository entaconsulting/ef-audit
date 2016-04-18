using System.Data.Entity.ModelConfiguration;

namespace Audit.Audit
{
    public class AuditTrailTypeConfiguration:EntityTypeConfiguration<AuditTrail>
    {
        public AuditTrailTypeConfiguration()
        {
            Property(p => p.Datos).HasColumnType("xml");
        }
    }
}
