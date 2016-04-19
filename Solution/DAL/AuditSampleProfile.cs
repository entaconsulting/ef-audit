using Audit.Audit;
using Dal.Sample.Model;

namespace DAL
{
    public class AuditSampleProfile : AuditProfile
    {
        public override void Configure()
        {
            base.Configure();

            AddAuditable<Usuario>(e => e.Id)
                .IgnoreIfNoFieldChanged()
                .AuditField(e => e.Nombre)
                .AuditField(e => e.Apellido)
                .AuditField(e => e.FechaNacimiento);

            

            AddAuditable<Pais>(e => e.Id);

        }
    }
}
