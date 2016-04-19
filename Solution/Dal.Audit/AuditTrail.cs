using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dal.Audit
{
    /// <summary>
    /// Class AuditTrail.
    /// </summary>
    public class AuditTrail
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the entidad.
        /// </summary>
        /// <value>The entidad.</value>
        public string Entidad { get; set; }
        /// <summary>
        /// Gets or sets the clave entidad.
        /// </summary>
        /// <value>The clave entidad.</value>
        public string ClaveEntidad { get; set; }
        /// <summary>
        /// Gets or sets the fecha update.
        /// </summary>
        /// <value>The fecha update.</value>
        public DateTime FechaUpdate { get; set; }
        /// <summary>
        /// Gets or sets the usuario.
        /// </summary>
        /// <value>The usuario.</value>
        public string Usuario { get; set; }
        /// <summary>
        /// Gets or sets the datos.
        /// </summary>
        /// <value>The datos.</value>
        [Column(TypeName = "xml")]
        public string Datos { get; set; }
        /// <summary>
        /// Gets or sets the composite key.
        /// </summary>
        /// <value>The composite key.</value>
        public string CompositeKey { get; set; }
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public string Action { get; set; }
    }
}