using System.Data.Entity.ModelConfiguration;

namespace Dal.Audit
{
    public class AuditTrailTypeConfiguration:EntityTypeConfiguration<AuditTrail>
    {
        public AuditTrailTypeConfiguration()
        {
            Property(p => p.Datos).HasColumnType("xml");
        }
    }
}
