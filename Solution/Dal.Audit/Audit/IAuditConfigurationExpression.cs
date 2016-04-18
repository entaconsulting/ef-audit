using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Audit.Audit
{
    public interface IAuditHierarchyTableConfigurationExpression<T>
    {
        IAuditHierarchyTableConfigurationExpression<T> AuditField(Expression<Func<T, object>> field);
        //IAuditHierarchyTableConfigurationExpression<T> AuditField(Expression<Func<T, object>> field, string fieldDescription, Dictionary<string, string> valuesDesc);
        IAuditHierarchyTableConfigurationExpression<T> AuditRelation<TRel>(Expression<Func<T, IEnumerable<TRel>>> relationCollection, Expression<Func<TRel, int>> fkField, Expression<Func<TRel, object>> auditableField, Expression<Func<TRel, object>> descriptionExpression) where TRel : class;
        IAuditHierarchyTableConfigurationExpression<T> AuditCompositeKey(Func<T, string> func);
        IAuditHierarchyTableConfigurationExpression<T> IgnoreIfNoFieldChanged();
        IAuditHierarchyTableConfigurationExpression<T> EntityName(string name);
    }
}