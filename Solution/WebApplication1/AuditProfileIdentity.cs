using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Audit.Audit;
using DAL;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication1.Models;

namespace WebApplication1
{
   

    public class AuditProfileIdentity : AuditProfile
    {
        public override void Configure()
        {
            base.Configure();

            AddAuditable<ApplicationUser>(e => e.Id)
                .IgnoreIfNoFieldChanged()
                .AuditField(e => e.Email)
                .AuditField(e => e.PasswordHash)
                .EntityName("ApplicationUserTest");


        }
    }
}