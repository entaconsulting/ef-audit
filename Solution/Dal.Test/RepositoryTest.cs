using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Base;
using Dal.Sample.Model;
using Dal.Test.Mocks;
using DAL;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Dal.Test
{
    //Inicializa una base de datos y ejecuta las operaciones básicas sobre el repositorio
    [TestFixture]
    public class RepositoryTest
    {
        private IUoW _uow;
        private readonly Fixture _fixture=new Fixture();

        [SetUp]
        public void Init()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<SampleDbContext>());
            var dbContext = new SampleDbContext("RepositoryTest");
            var repositoryFactory = new RepositoryFactoryMock(dbContext);
            _uow = new UoW(dbContext,repositoryFactory);
        }
        [Test]
        public async Task Add()
        {
            var usuario = _fixture.Create<Usuario>();
            usuario.Id = 0;

            var rUsuarios = _uow.GetRepository<Usuario>();

            rUsuarios.Add(usuario);
            await _uow.SaveChangesAsync();

            Assert.AreNotEqual(usuario.Id,0);


        }
        [Test]
        public async Task Update()
        {
            var usuario = _fixture.Create<Usuario>();
            usuario.Id = 0;
            var rUsuarios = _uow.GetRepository<Usuario>();
            rUsuarios.Add(usuario);
            _uow.SaveChangesAsync().Wait();

            usuario.Apellido = "test";
            rUsuarios.Update(usuario);
            await _uow.SaveChangesAsync();

            var usuario2 = await rUsuarios.GetByIdAsync(usuario.Id);

            Assert.AreEqual(usuario.Apellido, usuario2.Apellido);

        }
        [Test]
        public async Task Delete()
        {
            var usuario = _fixture.Create<Usuario>();
            usuario.Id = 0;
            var rUsuarios = _uow.GetRepository<Usuario>();
            rUsuarios.Add(usuario);
            _uow.SaveChangesAsync().Wait();

            rUsuarios.Delete(usuario);
            await _uow.SaveChangesAsync();

            Assert.Catch(async () =>
            {
                await rUsuarios.GetByIdAsync(usuario.Id);
            });

        }
    }
}
