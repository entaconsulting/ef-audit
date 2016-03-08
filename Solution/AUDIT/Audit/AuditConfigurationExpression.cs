using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Audit.Audit
{
    public class AuditHierarchyConfigurationExpression<T> : IAuditHierarchyTableConfigurationExpression<T>, IAuditHierarchyConfigurationExpression<T> where T : class
    {
        private readonly AuditProfile _parent;
        private readonly Type _entity;

        public AuditHierarchyConfigurationExpression(AuditProfile parent, Type entity)
        {
            _parent = parent;
            _entity = entity;
        }

        public IAuditHierarchyTableConfigurationExpression<T> AuditField(Expression<Func<T, Object>> field)
        {
            return AuditField(field, null, null);
        }

        public IAuditHierarchyTableConfigurationExpression<T> AuditField(Expression<Func<T, object>> field, string fieldDescription, Dictionary<string, string> valuesDesc)
        {
            var fieldName = Helpers.GetFullPropertyName(field);
            _parent.AddAuditableField<T>(fieldName, fieldDescription, valuesDesc);
            return this;
        }

        public IAuditHierarchyTableConfigurationExpression<T> AuditRelation<TRel>(Expression<Func<T, IEnumerable<TRel>>> relationCollection, Expression<Func<TRel, int>> fkField, Expression<Func<TRel, object>> auditableField, Expression<Func<TRel, object>> descriptionExpression) where TRel : class
        {
            var fieldName = Helpers.GetFullPropertyName(fkField);
            var auditableFieldName = Helpers.GetFullPropertyName(auditableField);
            var referenceCollectionName = Helpers.GetFullPropertyName(relationCollection);
            var descriptionMember = Helpers.GetMemberExpression(descriptionExpression);
            var descriptionFieldName = descriptionMember.Member.Name;
            Type descriptionPropertyType;
            if (descriptionMember.Expression is MemberExpression)
            {
                descriptionPropertyType = (descriptionMember.Expression as MemberExpression).Type;
            }
            else
            {
                descriptionPropertyType = descriptionMember.Type;
            }
            _parent.AddAuditableReference<TRel, T>(referenceCollectionName, fieldName, auditableFieldName, descriptionPropertyType, descriptionFieldName);
            return this;
        }


        public IAuditHierarchyTableConfigurationExpression<T> AuditCompositeKey(Func<T, string> func)
        {
            _parent.AddCompositeKey(func);
            return this;
        }

        public IAuditHierarchyTableConfigurationExpression<T> IgnoreIfNoFieldChanged()
        {
            _parent.SetIgnoreIfNoFieldChanged<T>();
            return this;
        }

        public IAuditHierarchyConfigurationExpression<T> ExcludeTable<TExclude>() where TExclude : T
        {
            _parent.Exclude(typeof(TExclude));
            return this;
        }

        public IAuditHierarchyConfigurationExpression<T> ExcludeSameNameSpaceOf<TExclude>() where TExclude : T
        {
            _parent.ExcludeNameSpace(typeof (TExclude).Namespace);
            return this;
        }
    }
}