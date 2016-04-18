namespace Audit.Audit
{
    public class AuditDataDetail:AuditData
    {
        public string Action { get; set; }
        public string NewValue { get; set; }
        public string ValueDescription { get; set; }
        public string FieldName { get; set; }
        public string FieldDescription { get; set; }
    }

    public class AuditDataDetailRelation : AuditData
    {
        public string Action { get; set; }
    }
}