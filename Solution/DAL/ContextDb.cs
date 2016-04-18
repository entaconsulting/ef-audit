using System;
using System.Data.Entity;
using Audit.Audit;
using Dal.Sample.Model;


namespace DAL
{

    public class SampleDbContext : DbContext
    {
        /// <summary>
        /// Constructor con lazy loading deshabilitado por default
        /// </summary>
        public SampleDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }

        public SampleDbContext() : base()
        {

        }


        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
    }

}
