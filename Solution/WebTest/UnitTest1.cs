using System;
using System.Linq;
using Audit;
using Audit.Audit;
using DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1;
using WebApplication1.Models;
using AppContext = DAL.AppContext;

namespace WebTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void WebTestCreateAsync()
        {

            var profile = new AuditProfileIdentity(); //deberia buidearse solo 1 vez y luego utilizarse siempre el mismo
            var appContext = new AppContext();
            var provider = new AuditProvider(new ContextDb());


            var proxyContext = (ApplicationDbContext)new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy(typeof(ApplicationDbContext), new AuditInterceptor(provider, profile, appContext));


            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(proxyContext));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
           

            var rnd = new Random().Next();
            var user = new ApplicationUser { UserName = $"alejandroslabkov{rnd}@gmail.com", Email = $"alejandroslabkov{rnd}@gmail.com" };


           
           var result = manager.CreateAsync(user, "@@asdAsd123");

            result.Wait();

            var userToUpdate = manager.Users.First();
            userToUpdate.Email = userToUpdate.Email + "X";
            manager.UpdateAsync(userToUpdate).Wait();

            var r = result.Result;


        }

    }

   
}
