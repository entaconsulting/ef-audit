using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dal.Audit
{
    /// <summary>
    /// Class AuditConfigurationEntry.
    /// </summary>
    public class AuditConfigurationEntry
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is auditable.
        /// </summary>
        /// <value><c>true</c> if this instance is auditable; otherwise, <c>false</c>.</value>
        public bool IsAuditable { get; set; }
        /// <summary>
        /// Gets or sets the auditable fields.
        /// </summary>
        /// <value>The auditable fields.</value>
        public IEnumerable<AuditFieldDefinition> AuditableFields { get; set; }
        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        /// <value>The entity key.</value>
        public LambdaExpression EntityKey { get; set; }
        /// <summary>
        /// Gets or sets the name of the entity key property.
        /// </summary>
        /// <value>The name of the entity key property.</value>
        public String EntityKeyPropertyName { get; set; }
        /// <summary>
        /// Gets or sets the auditable references.
        /// </summary>
        /// <value>The auditable references.</value>
        public IEnumerable<AuditConfigurationReferenceEntry> AuditableReferences { get; set; }
        /// <summary>
        /// Gets or sets the composite key.
        /// </summary>
        /// <value>The composite key.</value>
        public Func<object, string> CompositeKey { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [ignore if no field changed].
        /// </summary>
        /// <value><c>true</c> if [ignore if no field changed]; otherwise, <c>false</c>.</value>
        public bool IgnoreIfNoFieldChanged { get; set; }
        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>The name of the entity.</value>
        public string EntityName { get; set; }

        /// <summary>
        /// Class AuditConfigurationReferenceEntry.
        /// </summary>
        public class AuditConfigurationReferenceEntry
        {
            /// <summary>
            /// Gets or sets the type of the reference.
            /// </summary>
            /// <value>The type of the reference.</value>
            public Type ReferenceType { get; set; }
            /// <summary>
            /// Gets or sets the name of the reference property.
            /// </summary>
            /// <value>The name of the reference property.</value>
            public string ReferencePropertyName { get; set; }
            /// <summary>
            /// Gets or sets the name of the auditable property.
            /// </summary>
            /// <value>The name of the auditable property.</value>
            public string AuditablePropertyName { get; set; }
            /// <summary>
            /// Gets or sets the name of the reference collection.
            /// </summary>
            /// <value>The name of the reference collection.</value>
            public string ReferenceCollectionName { get; set; }
            /// <summary>
            /// Gets or sets the type of the description property.
            /// </summary>
            /// <value>The type of the description property.</value>
            public Type DescriptionPropertyType { get; set; }
            /// <summary>
            /// Gets or sets the name of the description property.
            /// </summary>
            /// <value>The name of the description property.</value>
            public string DescriptionPropertyName { get; set; }
        }
    }

}