using System;

namespace Dal.Audit
{
    /// <summary>
    /// Class AuditData.
    /// </summary>
    public class AuditData
    {
        /// <summary>
        /// Gets or sets the change date.
        /// </summary>
        /// <value>The change date.</value>
        public DateTime ChangeDate { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

    }
}