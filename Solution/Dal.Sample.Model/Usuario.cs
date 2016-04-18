using System;
using System.Collections.Generic;
using System.Data;
using Dal.Base;

namespace Dal.Sample.Model
{
    public class Usuario:EntityBase
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int PaisId { get; set; }
        public Pais Pais { get; set; }
        public ICollection<UsuarioEstadoHistory> Estados { get; set; }
    }
}
