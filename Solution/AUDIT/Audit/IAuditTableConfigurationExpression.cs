using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Audit.Audit
{
    public interface IAuditHierarchyConfigurationExpression<T> : IAuditHierarchyTableConfigurationExpression<T>
    {
        IAuditHierarchyConfigurationExpression<T> ExcludeTable<TExclude>() where TExclude:T;
        IAuditHierarchyConfigurationExpression<T> ExcludeSameNameSpaceOf<TExclude>() where TExclude : T;
    }
}