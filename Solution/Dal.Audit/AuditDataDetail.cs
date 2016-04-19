namespace Dal.Audit
{
    /// <summary>
    /// Class AuditDataDetail.
    /// </summary>
    /// <seealso cref="Dal.Audit.AuditData" />
    public class AuditDataDetail:AuditData
    {
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public string Action { get; set; }
        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public string NewValue { get; set; }
        /// <summary>
        /// Gets or sets the value description.
        /// </summary>
        /// <value>The value description.</value>
        public string ValueDescription { get; set; }
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName { get; set; }
        /// <summary>
        /// Gets or sets the field description.
        /// </summary>
        /// <value>The field description.</value>
        public string FieldDescription { get; set; }
    }

    /// <summary>
    /// Class AuditDataDetailRelation.
    /// </summary>
    /// <seealso cref="Dal.Audit.AuditData" />
    public class AuditDataDetailRelation : AuditData
    {
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public string Action { get; set; }
    }
}