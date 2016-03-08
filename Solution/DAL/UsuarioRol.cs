using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UsuarioRol
    {
        public int Id { get; set; }
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public Rol Rol { get; set; }
        public int RolId { get; set; }
    }
}
