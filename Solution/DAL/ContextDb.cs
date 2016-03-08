using System;
using System.Data.Entity;
using Audit.Audit;


namespace DAL
{

    public class ContextDb : DbContext
    {
        /// <summary>
        /// Constructor con lazy loading deshabilitado por default
        /// </summary>
        public ContextDb(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }

        public ContextDb() : base()
        {

        }


        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
    }

}
