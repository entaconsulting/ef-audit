using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dal.Audit
{
    /// <summary>
    /// Class AuditHierarchyConfigurationExpression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Dal.Audit.IAuditHierarchyTableConfigurationExpression{T}" />
    /// <seealso cref="Dal.Audit.IAuditHierarchyConfigurationExpression{T}" />
    public class AuditHierarchyConfigurationExpression<T> : IAuditHierarchyTableConfigurationExpression<T>, IAuditHierarchyConfigurationExpression<T> where T : class
    {
        private readonly AuditProfile _parent;
        private readonly Type _entity;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditHierarchyConfigurationExpression{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="entity">The entity.</param>
        public AuditHierarchyConfigurationExpression(AuditProfile parent, Type entity)
        {
            _parent = parent;
            _entity = entity;
        }

        /// <summary>
        /// Audits the field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        public IAuditHierarchyTableConfigurationExpression<T> AuditField(Expression<Func<T, Object>> field)
        {
            return AuditField(field, null, null);
        }

        /// <summary>
        /// Audits the field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="fieldDescription">The field description.</param>
        /// <param name="valuesDesc">The values desc.</param>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        public IAuditHierarchyTableConfigurationExpression<T> AuditField(Expression<Func<T, object>> field, string fieldDescription, Dictionary<string, string> valuesDesc)
        {
            var fieldName = Helpers.GetFullPropertyName(field);
            _parent.AddAuditableField<T>(fieldName, fieldDescription, valuesDesc);
            return this;
        }

        /// <summary>
        /// Audits the relation.
        /// </summary>
        /// <typeparam name="TRel">The type of the t relative.</typeparam>
        /// <param name="relationCollection">The relation collection.</param>
        /// <param name="fkField">The fk field.</param>
        /// <param name="auditableField">The auditable field.</param>
        /// <param name="descriptionExpression">The description expression.</param>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
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


        /// <summary>
        /// Audits the composite key.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        public IAuditHierarchyTableConfigurationExpression<T> AuditCompositeKey(Func<T, string> func)
        {
            _parent.AddCompositeKey(func);
            return this;
        }

        /// <summary>
        /// Ignores if no field changed.
        /// </summary>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        public IAuditHierarchyTableConfigurationExpression<T> IgnoreIfNoFieldChanged()
        {
            _parent.SetIgnoreIfNoFieldChanged<T>();
            return this;
        }

        /// <summary>
        /// Excludes the table.
        /// </summary>
        /// <typeparam name="TExclude">The type of the t exclude.</typeparam>
        /// <returns>IAuditHierarchyConfigurationExpression&lt;T&gt;.</returns>
        public IAuditHierarchyConfigurationExpression<T> ExcludeTable<TExclude>() where TExclude : T
        {
            _parent.Exclude(typeof(TExclude));
            return this;
        }

        /// <summary>
        /// Excludes the same name space of.
        /// </summary>
        /// <typeparam name="TExclude">The type of the t exclude.</typeparam>
        /// <returns>IAuditHierarchyConfigurationExpression&lt;T&gt;.</returns>
        public IAuditHierarchyConfigurationExpression<T> ExcludeSameNameSpaceOf<TExclude>() where TExclude : T
        {
            _parent.ExcludeNameSpace(typeof (TExclude).Namespace);
            return this;
        }

        /// <summary>
        /// Entities the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IAuditHierarchyTableConfigurationExpression&lt;T&gt;.</returns>
        public IAuditHierarchyTableConfigurationExpression<T> EntityName(string name)
        {
            _parent.EntityName<T>(name);
            return this;
        }

    }
}