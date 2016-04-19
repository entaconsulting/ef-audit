namespace Dal.Audit
{
    /// <summary>
    /// Interface IAuditHierarchyConfigurationExpression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Dal.Audit.IAuditHierarchyTableConfigurationExpression{T}" />
    public interface IAuditHierarchyConfigurationExpression<T> : IAuditHierarchyTableConfigurationExpression<T>
    {
        /// <summary>
        /// Excludes the table.
        /// </summary>
        /// <typeparam name="TExclude">The type of the t exclude.</typeparam>
        /// <returns>IAuditHierarchyConfigurationExpression&lt;T&gt;.</returns>
        IAuditHierarchyConfigurationExpression<T> ExcludeTable<TExclude>() where TExclude:T;
        /// <summary>
        /// Excludes the same name space of.
        /// </summary>
        /// <typeparam name="TExclude">The type of the t exclude.</typeparam>
        /// <returns>IAuditHierarchyConfigurationExpression&lt;T&gt;.</returns>
        IAuditHierarchyConfigurationExpression<T> ExcludeSameNameSpaceOf<TExclude>() where TExclude : T;
    }
}