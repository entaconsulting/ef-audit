using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dal.Audit
{
    public class AuditTrail
    {
        public int Id { get; set; }
        public string Entidad { get; set; }
        public string ClaveEntidad { get; set; }
        public DateTime FechaUpdate { get; set; }
        public string Usuario { get; set; }
        [Column(TypeName = "xml")]
        public string Datos { get; set; }
        public string CompositeKey { get; set; }
        public string Action { get; set; }
    }
}