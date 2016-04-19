using System.Data.Entity.ModelConfiguration;

namespace Dal.Audit
{
    /// <summary>
    /// Class AuditTrailTypeConfiguration.
    /// </summary>
    /// <seealso cref="System.Data.Entity.ModelConfiguration.EntityTypeConfiguration{Dal.Audit.AuditTrail}" />
    public class AuditTrailTypeConfiguration:EntityTypeConfiguration<AuditTrail>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditTrailTypeConfiguration"/> class.
        /// </summary>
        public AuditTrailTypeConfiguration()
        {
            Property(p => p.Datos).HasColumnType("xml");
        }
    }
}
