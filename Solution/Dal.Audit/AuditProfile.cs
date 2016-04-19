using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dal.Audit
{
    /// <summary>
    /// Class AuditProfile.
    /// </summary>
    public abstract class AuditProfile 
    {
        private const string DynamicProxyAssemblyName = "System.Data.Entity.DynamicProxies";
        private readonly IDictionary<Type, AuditConfiguration> _auditList;
//        private readonly IDictionary<Type, AuditConfiguration> _auditAllList;
        private readonly HashSet<Type> _excludeTypeList;
        private readonly HashSet<string> _excludeNsList;
        private bool _isInitialized;
        protected AuditProfile()
        {
            _auditList = new Dictionary<Type, AuditConfiguration>();
//            _auditAllList = new Dictionary<Type, AuditConfiguration>();
            _excludeTypeList = new HashSet<Type>();
            _excludeNsList = new HashSet<string>();
            _isInitialized = false;
        }

        /// <summary>
        /// Configures this instance.
        /// </summary>
        public virtual void Configure()
        {
            _isInitialized = true;
        }

        protected IAuditHierarchyTableConfigurationExpression<T> AddAuditable<T>(Expression<Func<T, object>> uniqueId) where T : class
        {
            AuditConfiguration entityConfig;
            if (!_auditList.TryGetValue(typeof(T), out entityConfig))
            {
                entityConfig = new AuditConfiguration()
                {
                    AuditFields = new List<AuditFieldDefinition>(),
                    AuditReferences = new List<AuditConfigurationEntry.AuditConfigurationReferenceEntry>()
                };
                _auditList.Add(typeof(T), entityConfig);
            }
            if (uniqueId != null)
            {
                entityConfig.EntityKey = uniqueId;
                entityConfig.EntityKeyName = Helpers.GetFullPropertyName(uniqueId);
            }
            var configurationExpression = new AuditHierarchyConfigurationExpression<T>(this, typeof(T));
            return configurationExpression;
        }

        protected IAuditHierarchyTableConfigurationExpression<T> GetAuditable<T>() where T : class
        {
            if (!_auditList.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException(string.Format("Tipo '{0}' no mapeado",typeof(T).FullName));
            }
            var configurationExpression = new AuditHierarchyConfigurationExpression<T>(this, typeof(T));
            return configurationExpression;
        }

        protected IAuditHierarchyConfigurationExpression<T> AuditAllOfType<T>(Expression<Func<T, object>> uniqueId) where T : class
        {
            AuditConfiguration entityConfig;
            if (!_auditList.TryGetValue(typeof(T), out entityConfig))
            {
                entityConfig = new AuditConfiguration()
                {
                    AuditFields = new List<AuditFieldDefinition>(),
                    AuditReferences = new List<AuditConfigurationEntry.AuditConfigurationReferenceEntry>()
                };
                _auditList.Add(typeof(T), entityConfig);
            }
            if (uniqueId != null)
            {
                entityConfig.EntityKey = uniqueId;
                entityConfig.EntityKeyName = Helpers.GetFullPropertyName(uniqueId);
            }
            entityConfig.Generic = true;

            var configurationExpression = new AuditHierarchyConfigurationExpression<T>(this, typeof(T));
            return configurationExpression;

        }

        /// <summary>
        /// Excludes the specified exclude.
        /// </summary>
        /// <param name="exclude">The exclude.</param>
        public void Exclude(Type exclude)
        {
            if (!_excludeTypeList.Contains(exclude))
            {
                _excludeTypeList.Add(exclude);
            }
        }

        /// <summary>
        /// Excludes the name space.
        /// </summary>
        /// <param name="ns">The ns.</param>
        public void ExcludeNameSpace(string ns)
        {
            if (!_excludeNsList.Contains(ns))
            {
                _excludeNsList.Add(ns);
            }
        }


        internal void AddAuditableField<T>(string fieldName, string fieldDescripcion, Dictionary<string, string> valuesConverter)
        {
            var entityConfig = _auditList[typeof(T)];
            entityConfig.AuditFields.Add(new AuditFieldDefinition() { FieldName = fieldName, FieldDescription = fieldDescripcion });
        }

        internal void AddAuditableReference<T, TRef>(string referenceCollectionName, string fieldName, string auditableFieldName, Type descriptionFieldType, string descriptionFieldName)
            where T : class
            where TRef : class
        {
            AuditConfiguration entityConfig;
            if (!_auditList.TryGetValue(typeof(T), out entityConfig))
            {
                AddAuditable<T>(null);
                entityConfig = _auditList[typeof(T)];
            }
            entityConfig.AuditReferences.Add(new AuditConfigurationEntry.AuditConfigurationReferenceEntry()
            {
                ReferenceCollectionName = referenceCollectionName,
                ReferencePropertyName = fieldName,
                DescriptionPropertyType = descriptionFieldType,
                DescriptionPropertyName = descriptionFieldName,
                AuditablePropertyName = auditableFieldName,
                ReferenceType = typeof(TRef)
            });

        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>AuditConfigurationEntry.</returns>
        public AuditConfigurationEntry GetConfiguration(Type entityType)
        {
            if (!_isInitialized)
            {
                Configure();
            }

            var entry = new AuditConfigurationEntry() { IsAuditable = false };
            var configuration = GetConfigurationFromType(entityType);

            if (configuration == null)
                return entry;

            entry.IsAuditable = true;
            entry.EntityKey = configuration.EntityKey;
            entry.EntityKeyPropertyName = configuration.EntityKeyName;
            entry.AuditableFields = configuration.AuditFields;
            entry.AuditableReferences = configuration.AuditReferences;
            entry.CompositeKey = configuration.CompositeKeyFunc;
            entry.IgnoreIfNoFieldChanged = configuration.IgnoreIfNoFieldChanged;
            entry.EntityName = configuration.EntityName;
            return entry;
        }

        private AuditConfiguration GetConfigurationFromType(Type entityType)
        {
            //chequeo si el tipo es un proxy de EF en cuyo caso tomo el basetype
            var internalEntityType = entityType.FullName.StartsWith(DynamicProxyAssemblyName)
                ? entityType.BaseType
                : entityType;

            AuditConfiguration configuration;
            if (!_auditList.TryGetValue(internalEntityType, out configuration))
            {
                //busco alguna configuración genérica para el tipo
                //me quedo con la más específica de las que hay
                var genericConfigurations = _auditList
                    .Where(a => a.Value.Generic &&  a.Key.IsAssignableFrom(internalEntityType) && !_excludeTypeList.Contains(internalEntityType) && !_excludeNsList.Contains(internalEntityType.Namespace))
                    .ToList();
                if (genericConfigurations.Any())
                {
                    configuration = genericConfigurations
                        .Aggregate((pv, cv) => pv.Key.IsAssignableFrom(cv.Key) ? cv : pv)
                        .Value;
                }
            }
            return configuration;
        }

        /// <summary>
        /// Adds the composite key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The function.</param>
        public void AddCompositeKey<T>(Func<T, string> func) where T : class
        {
            _auditList[typeof(T)].CompositeKeyFunc = (o) => func(o as T);
        }

        /// <summary>
        /// Sets the ignore if no field changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SetIgnoreIfNoFieldChanged<T>()
        {
            _auditList[typeof (T)].IgnoreIfNoFieldChanged = true;
        }

        /// <summary>
        /// Entities the name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        public void EntityName<T>(string name)
        {
            _auditList[typeof(T)].EntityName = name;
        }
    }
}
