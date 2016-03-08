using Audit.Audit;

namespace DAL
{
    public class AuditCustomProfile : AuditProfile
    {
        public override void Configure()
        {
            base.Configure();

            AddAuditable<Usuario>(e => e.Id)
                .IgnoreIfNoFieldChanged()
                .AuditField(e => e.Nombre)
                .AuditField(e => e.Apellido)
                .AuditField(e => e.Habilitado)
                .AuditField(e => e.FechaNacimiento);

            

            AddAuditable<Pais>(e => e.Id);

        }
    }
}
