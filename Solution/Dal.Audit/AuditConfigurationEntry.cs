using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dal.Audit
{
    public class AuditConfigurationEntry
    {
        public bool IsAuditable { get; set; }
        public IEnumerable<AuditFieldDefinition> AuditableFields { get; set; }
        public LambdaExpression EntityKey { get; set; }
        public String EntityKeyPropertyName { get; set; }
        public IEnumerable<AuditConfigurationReferenceEntry> AuditableReferences { get; set; }
        public Func<object, string> CompositeKey { get; set; }
        public bool IgnoreIfNoFieldChanged { get; set; }
        public string EntityName { get; set; }

        public class AuditConfigurationReferenceEntry
        {
            public Type ReferenceType { get; set; }
            public string ReferencePropertyName { get; set; }
            public string AuditablePropertyName { get; set; }
            public string ReferenceCollectionName { get; set; }
            public Type DescriptionPropertyType { get; set; }
            public string DescriptionPropertyName { get; set; }
        }
    }

}