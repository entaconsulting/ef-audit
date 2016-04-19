using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dal.Audit
{
    /// <summary>
    /// Interface IAuditHierarchyTableConfigurationExpression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAuditHierarchyTableConfigurationExpression<T>
    {
        /// <summary>
        /// Audits the field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        IAuditHierarchyTableConfigurationExpression<T> AuditField(Expression<Func<T, object>> field);
        //IAuditHierarchyTableConfigurationExpression<T> AuditField(Expression<Func<T, object>> field, string fieldDescription, Dictionary<string, string> valuesDesc);
        /// <summary>
        /// Audits the relation.
        /// </summary>
        /// <typeparam name="TRel">The type of the t relative.</typeparam>
        /// <param name="relationCollection">The relation collection.</param>
        /// <param name="fkField">The fk field.</param>
        /// <param name="auditableField">The auditable field.</param>
        /// <param name="descriptionExpression">The description expression.</param>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        IAuditHierarchyTableConfigurationExpression<T> AuditRelation<TRel>(Expression<Func<T, IEnumerable<TRel>>> relationCollection, Expression<Func<TRel, int>> fkField, Expression<Func<TRel, object>> auditableField, Expression<Func<TRel, object>> descriptionExpression) where TRel : class;
        /// <summary>
        /// Audits the composite key.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        IAuditHierarchyTableConfigurationExpression<T> AuditCompositeKey(Func<T, string> func);
        /// <summary>
        /// Ignores if no field changed.
        /// </summary>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        IAuditHierarchyTableConfigurationExpression<T> IgnoreIfNoFieldChanged();
        /// <summary>
        /// Entities the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        IAuditHierarchyTableConfigurationExpression<T> EntityName(string name);
    }
}