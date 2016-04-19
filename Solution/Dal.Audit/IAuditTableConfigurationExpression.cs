namespace Dal.Audit
{
    public interface IAuditHierarchyConfigurationExpression<T> : IAuditHierarchyTableConfigurationExpression<T>
    {
        IAuditHierarchyConfigurationExpression<T> ExcludeTable<TExclude>() where TExclude:T;
        IAuditHierarchyConfigurationExpression<T> ExcludeSameNameSpaceOf<TExclude>() where TExclude : T;
    }
}