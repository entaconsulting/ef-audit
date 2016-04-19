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
using Dal.History;

namespace Dal.Test
{
    //Inicializa una base de datos y ejecuta las operaciones básicas sobre el repositorio
    [TestFixture]
    public class HistoryTest
    {
        private IUoW _uow;
        private readonly Fixture _fixture=new Fixture();
        private IRepository<Usuario> _rUsuarios;

        [SetUp]
        public void Init()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<SampleDbContext>());
            var dbContext = new SampleDbContext("HistoryTest");
            var repositoryFactory = new RepositoryFactoryMock(dbContext);
            _uow = new UoW(dbContext,repositoryFactory);

            //creo una nueva entidad para las pruebas de historicidad
            _rUsuarios = _uow.GetRepository<Usuario>();
        }

        [Test]
        public void CrearPrimeraInstancia()
        {
            var usuario = _fixture.Create<Usuario>();
            _rUsuarios.Add(usuario);
            _uow.SaveChanges();

            var hoy = DateTime.Today;
            var estado = usuario.CreateVersion(u => u.Estados, hoy, _uow);

            Assert.AreEqual(hoy, estado.VigenciaDesde);
            Assert.AreEqual(HistoryExtensions.MaxDate, estado.VigenciaHasta);
        }
        [Test]
        public async Task CrearNuevaVersionSobreLaVigente()
        {
            var usuario = _fixture.Create<Usuario>();
            _rUsuarios.Add(usuario);
            _uow.SaveChanges();

            var hoy = DateTime.Today;
            var estadoHoy = usuario.CreateVersion(u => u.Estados, hoy, _uow);
            estadoHoy.Habilitado = true;
            await _uow.SaveChangesAsync();

            var manana = hoy.AddDays(1);
            var estadoManana = usuario.CreateVersion(u => u.Estados, manana, _uow);
            estadoManana.Habilitado = false;
            await _uow.SaveChangesAsync();

            Assert.AreEqual(hoy, estadoHoy.VigenciaDesde);
            Assert.AreEqual(hoy, estadoHoy.VigenciaHasta);

            Assert.AreEqual(manana, estadoManana.VigenciaDesde);
            Assert.AreEqual(HistoryExtensions.MaxDate, estadoManana.VigenciaHasta);
        }
        [Test]
        public async Task CrearNuevaVersionSobreUnaNoVigente()
        {
            var usuario = _fixture.Create<Usuario>();
            _rUsuarios.Add(usuario);
            _uow.SaveChanges();

            var hoy = DateTime.Today;
            var estadoHoy = usuario.CreateVersion(u => u.Estados, hoy, _uow);
            estadoHoy.Habilitado = true;
            await _uow.SaveChangesAsync();

            var futuro = hoy.AddMonths(1);
            var estadoFuturo = usuario.CreateVersion(u => u.Estados, futuro, _uow);
            estadoFuturo.Habilitado = true;
            await _uow.SaveChangesAsync();

            var entreHoyYFuturo = hoy.AddDays(15);
            var estadoIntermedio = usuario.CreateVersion(u => u.Estados, entreHoyYFuturo, _uow);
            estadoIntermedio.Habilitado = false;
            await _uow.SaveChangesAsync();

            Assert.AreEqual(hoy, estadoHoy.VigenciaDesde);
            Assert.AreEqual(entreHoyYFuturo.AddDays(-1), estadoHoy.VigenciaHasta);

            Assert.AreEqual(entreHoyYFuturo, estadoIntermedio.VigenciaDesde);
            Assert.AreEqual(futuro.AddDays(-1), estadoIntermedio.VigenciaHasta);

            Assert.AreEqual(futuro, estadoFuturo.VigenciaDesde);
            Assert.AreEqual(HistoryExtensions.MaxDate, estadoFuturo.VigenciaHasta);
        }

        [Test]
        public async Task ObtenerVersionInexistente()
        {
            var hoy = DateTime.Today;
            var usuario = await GenerarUsuarioConHistoria(hoy);

            var ayer = hoy.AddDays(-1);
            var estadoAyer = usuario.GetVersion<Usuario, UsuarioEstadoHistory>(ayer, _uow);

            Assert.Null(estadoAyer);

        }

        [Test]
        public async Task ObtenerVersionInicial()
        {
            var hoy = DateTime.Today;
            var usuario = await GenerarUsuarioConHistoria(hoy);

            var estadoHoy = usuario.GetVersion<Usuario, UsuarioEstadoHistory>(hoy, _uow);

            Assert.AreEqual(hoy, estadoHoy.VigenciaDesde);

        }

        [Test]
        public async Task ObtenerVersionIntermedia()
        {
            var hoy = DateTime.Today;
            var usuario = await GenerarUsuarioConHistoria(hoy);

            //inicia 15 días después de hoy, pido 20 días pero me tiene que venir la versión que inicia en 15
            var vigenciaIntermedia = hoy.AddDays(20);
            var estado = usuario.GetVersion<Usuario, UsuarioEstadoHistory>(vigenciaIntermedia, _uow);

            Assert.AreEqual(hoy.AddDays(15), estado.VigenciaDesde);

        }

        private async Task<Usuario> GenerarUsuarioConHistoria(DateTime hoy)
        {
            var usuario = _fixture.Create<Usuario>();
            _rUsuarios.Add(usuario);
            _uow.SaveChanges();

            var estadoHoy = usuario.CreateVersion(u => u.Estados, hoy, _uow);
            estadoHoy.Habilitado = true;
            await _uow.SaveChangesAsync();

            var futuro = hoy.AddMonths(1);
            var estadoFuturo = usuario.CreateVersion(u => u.Estados, futuro, _uow);
            estadoFuturo.Habilitado = true;
            await _uow.SaveChangesAsync();

            var entreHoyYFuturo = hoy.AddDays(15);
            var estadoIntermedio = usuario.CreateVersion(u => u.Estados, entreHoyYFuturo, _uow);
            estadoIntermedio.Habilitado = false;
            await _uow.SaveChangesAsync();

            return usuario;

        }

    }

}
