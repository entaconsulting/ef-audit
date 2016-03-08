using Audit.Audit;
using DAL;
using WebApplication1.Models;

namespace WebApplication1
{
    public class AuditCustomProfile : AuditProfile
    {
        public override void Configure()
        {
            base.Configure();

            AddAuditable<ApplicationUser>(e => e.Id)
                .IgnoreIfNoFieldChanged()
                .AuditField(e => e.PasswordHash);



        }
    }
}
