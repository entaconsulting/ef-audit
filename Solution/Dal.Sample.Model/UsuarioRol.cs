using Dal.Base;

namespace Dal.Sample.Model
{
    public class UsuarioRol:EntityBase
    {
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public Rol Rol { get; set; }
        public int RolId { get; set; }
    }
}
