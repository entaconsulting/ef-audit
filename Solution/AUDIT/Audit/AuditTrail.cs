using System;

namespace Audit.Audit
{
    public class AuditTrail
    {
        public int Id { get; set; }
        public string Entidad { get; set; }
        public string ClaveEntidad { get; set; }
        public DateTime FechaUpdate { get; set; }
        public string Usuario { get; set; }
        public string Datos { get; set; }
        public string CompositeKey { get; set; }
        public string Action { get; set; }
    }
}