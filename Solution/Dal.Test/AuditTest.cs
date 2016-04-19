using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Audit;
using Dal.Base;
using Dal.Sample.Model;
using Dal.Test.Mocks;
using DAL;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Dal.History;

namespace Dal.Test
{
    //Inicializa una base de datos y ejecuta las operaciones básicas sobre el repositorio
    [TestFixture]
    public class AuditTest
    {
        private IUoW _uow;
        private readonly Fixture _fixture=new Fixture();
        private IRepository<Usuario> _rUsuarios;
        private IAuditManager _auditManager;

        [SetUp]
        public void Init()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<SampleDbContext>());
            var dbContext = new SampleDbContext("AuditTest");

            //configuro la auditoría sobre dbContext
            //utilizo el mismo dbcontext para guardar la auditoría, aunque podría ser uno distinto
            var auditProvider = new AuditProvider(dbContext);
            var auditProfile = new AuditSampleProfile();
            var appContext = new AuditTestAppContextMock();
            _auditManager = new AuditManager(auditProvider,auditProfile,appContext);
            var auditableDbContext = dbContext.BeginAudit(_auditManager);

            var repositoryFactory = new RepositoryFactoryMock(auditableDbContext);
            _uow = new UoW(auditableDbContext, repositoryFactory);

            //instancion repositorio de usuarios
            _rUsuarios = _uow.GetRepository<Usuario>();
        }

        [Test]
        public void AuditAdd()
        {
            var usuario = _fixture.Create<Usuario>();
            _rUsuarios.Add(usuario);
            _uow.SaveChanges();


            var data = _auditManager.GetFieldHistory(usuario, u => u.Nombre).SingleOrDefault();

            Assert.NotNull(data);
            Assert.AreEqual("Added", data.Action);
            Assert.AreEqual(usuario.Nombre, data.NewValue);

        }

        [Test]
        public void AuditUpdate()
        {
            var usuario = _fixture.Create<Usuario>();
            _rUsuarios.Add(usuario);
            _uow.SaveChanges();
            usuario.Nombre += "-Updated";
            _uow.SaveChanges();

            var trail = _auditManager.GetFieldHistory(usuario, u => u.Nombre).ToList();

            Assert.AreEqual(2, trail.Count);
            Assert.AreEqual("Added", trail[0].Action);
            Assert.AreEqual("Modified", trail[1].Action);
            Assert.AreEqual(usuario.Nombre, trail[1].NewValue);

        }

        [Test]
        public void AuditDelete()
        {
            var usuario = _fixture.Create<Usuario>();
            _rUsuarios.Add(usuario);
            _uow.SaveChanges();
            _rUsuarios.Delete(usuario);
            _uow.SaveChanges();

            var trail = _auditManager.GetFieldHistory(usuario).ToList();

            Assert.AreEqual(2, trail.Count);
            Assert.AreEqual("Added", trail[0].Action);
            Assert.AreEqual("Deleted", trail[1].Action);

        }

    }

}
