using Dal.Audit;
using Dal.Base;
using Dal.Sample.Model;

namespace DAL
{
    public class AuditSampleProfile : AuditProfile
    {
        public override void Configure()
        {
            base.Configure();

            //auditar todas las entidades que heredan de BaseEntity sólo a nivel entidad, sin detalle por campos
            AuditAllOfType<EntityBase>(e => e.Id)
                .IgnoreIfNoFieldChanged();

            //en el caso de usuario, auditar a nivel campo cambios en Nombre y Fecha de nacimiento
            AddAuditable<Usuario>(e => e.Id)
                .IgnoreIfNoFieldChanged()
                .AuditField(e => e.Nombre)
                .AuditField(e => e.FechaNacimiento);
        }
    }
}
