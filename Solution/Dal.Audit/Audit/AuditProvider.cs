using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Audit.Audit
{
    public class AuditProvider : IAuditProvider
    {
        private readonly DbContext _dbContext;
        private const string DynamicProxyAssemblyName = "System.Data.Entity.DynamicProxies";

        public AuditProvider(DbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public IEnumerable<AuditDataDetail> GetAuditTrail<T>(T entity, AuditConfigurationEntry config,
            params Expression<Func<T, object>>[] fields) where T : class
        {
            return GetAuditTrail(GetEntityName(entity), GetEntityKey(entity, config.EntityKey), fields.Select(Helpers.GetFullPropertyName).ToArray());
        }

        public IEnumerable<AuditDataDetail> GetAuditTrail(string entityName, string entityKey, params string[] fields)
        {
            var fieldsFilter = "";
            var root = "/changeSet";
            foreach (var field in fields)
            {
                if (fieldsFilter != "")
                {
                    fieldsFilter += ",";
                }
                fieldsFilter += root + "/" + field;
            }
            if (string.IsNullOrEmpty(fieldsFilter))
            {
                fieldsFilter = root;
            }

            var sql = string.Format(@"select fechaUpdate as changeDate, 
                                usuario as userName,
                                change.field.query('local-name(.)') as fieldName,
                                change.field.query('data(./description)') as newValue,
                                change.field.query('data(./propertyDescription)') as fieldDescription,
                                change.field.query('data(./valueDescription)') as valueDescription,
	                            Action as action
                        from audittrails cross apply datos.nodes('{2}') as change(field)
                        where   ClaveEntidad = '{0}'
                                AND Entidad = '{1}'
                                AND datos.exist('{2}')=1", entityKey, entityName, fieldsFilter);
            return _dbContext.Database.SqlQuery<AuditDataDetail>(sql);
        }

        public IEnumerable<AuditDataDetail> GetAuditTrailByCompositeKey(string entityName, string compositeKey, params string[] fields)
        {
            var fieldsFilter = "";
            var root = "/changeSet";
            foreach (var field in fields)
            {
                if (fieldsFilter != "")
                {
                    fieldsFilter += ",";
                }
                fieldsFilter += root + "/" + field;
            }
            if (string.IsNullOrEmpty(fieldsFilter))
            {
                fieldsFilter = root;
            }

            var sql = string.Format(@"select fechaUpdate as changeDate, 
                                usuario as userName,
                                change.field.query('local-name(.)') as fieldName,
                                change.field.query('data(./description)') as newValue,
                                change.field.query('data(./propertyDescription)') as fieldDescription,
                                change.field.query('data(./valueDescription)') as valueDescription,
	                            Action as action
                        from audittrails cross apply datos.nodes('{2}') as change(field)
                        where   CompositeKey = '{0}'
                                AND Entidad = '{1}'
                                AND datos.exist('{2}')=1", compositeKey, entityName, fieldsFilter);
            return _dbContext.Database.SqlQuery<AuditDataDetail>(sql);
        }

        public IEnumerable<AuditDataDetailRelation> GetAuditTrailByCompositeKeyRelation(string entityName, string compositeKey)
        {

            var sql = string.Format(@"select fechaUpdate as changeDate, 
                                usuario as userName,
                                action as action
                        from audittrails
                        where   CompositeKey = '{0}'
                                AND Entidad = '{1}'", compositeKey, entityName);
            return _dbContext.Database.SqlQuery<AuditDataDetailRelation>(sql);
        }

        public void Write()
        {
            _dbContext.SaveChanges();
        }

        public async Task WriteAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void AddAudit(DbEntityEntry entry, DateTime date, string user, AuditConfigurationEntry config, EntityState writeMode)
        {

            var compositeKey = string.Empty;

            if (config.CompositeKey != null)
            {
                compositeKey = config.CompositeKey.Invoke(entry.Entity);
            }

            //Auditoria de la entidad y sus campos
            if (config.EntityKey != null)
            {
                bool fieldsChanged;
                var auditTrailEntry = new AuditTrail
                {
                    Entidad = string.IsNullOrEmpty(config.EntityName) ? GetEntityName(entry.Entity) : config.EntityName,
                    ClaveEntidad = GetEntityKey(entry.Entity, config.EntityKey),
                    FechaUpdate = date,
                    Usuario = user,
                    CompositeKey = compositeKey,
                    Datos = GetXml(entry, config.AuditableFields, writeMode, out fieldsChanged),
                    Action = writeMode.ToString()
                };
                //chequeo si tengo que ignorar la auditoría porque no cambió ningún campo auditable
                if (!config.IgnoreIfNoFieldChanged || writeMode != EntityState.Modified || fieldsChanged)
                {
                    _dbContext.Set<AuditTrail>().Add(auditTrailEntry);
                }
            }
            //Auditoria de referencias. sólo para add o remove de la referencia
            //TODO: implementar modificación, importa cuando lo que se modifica es la fk no el resto
            foreach (var referenceConfig in config.AuditableReferences)
            {
                var desc = GetXmlReference(entry, referenceConfig.ReferenceCollectionName, referenceConfig.AuditablePropertyName, referenceConfig.DescriptionPropertyType, referenceConfig.DescriptionPropertyName);
                if (!string.IsNullOrEmpty(desc))
                {
                    var auditTrailReferenceEntry = new AuditTrail
                    {
                        Entidad = GetNameFromType(referenceConfig.ReferenceType),
                        ClaveEntidad = GetReferenceKey(entry, referenceConfig.ReferencePropertyName),
                        FechaUpdate = date,
                        Usuario = user,
                        CompositeKey = compositeKey,
                        Datos = desc
                    };
                    _dbContext.Set<AuditTrail>().Add(auditTrailReferenceEntry);
                }
            }
        }

        private string GetReferenceKey(DbEntityEntry entry, string referencePropertyName)
        {
            var property = entry.Property(referencePropertyName);
            var value = property.CurrentValue.ToString();
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("No se ha encontrado el valor de clave de la referencia");
            }
            return value;
        }

        private string GetEntityName(object entity)
        {
            return GetNameFromType(entity.GetType());
        }

        private string GetNameFromType(Type type)
        {
            //chequeo si el tipo es un proxy de EF en cuyo caso tomo el basetype
            var internalType = type.FullName.StartsWith(DynamicProxyAssemblyName)
                ? type.BaseType
                : type;

            return internalType.Name;

        }
        private string GetEntityKey(object entity, LambdaExpression entityKey)
        {
            var memberExpression = Helpers.GetMemberExpression(entityKey);
            if (memberExpression == null)
                throw new InvalidOperationException("No se puede obtener la clave de la entidad");
            return (memberExpression.Member as PropertyInfo).GetValue(entity).ToString();
        }


        private static string GetXml(DbEntityEntry entry, IEnumerable<AuditFieldDefinition> auditableFields, EntityState stateToBuild, out bool fieldsChanged)
        {
            var xml = new StringBuilder();
            var settings = new XmlWriterSettings { Indent = true, IndentChars = "\t", NewLineOnAttributes = true };
            fieldsChanged = false;
            using (XmlWriter writer = XmlWriter.Create(xml, settings))
            {
                writer.WriteStartElement("changeSet");
                //tengo que recuperar los datos de la BD porque la entidad puede no tener los original values seteados
                if (auditableFields.Any())
                {
                    if (stateToBuild != EntityState.Added)
                    {
                        var dbValues = entry.GetDatabaseValues();
                        foreach (var auditableField in auditableFields)
                        {
                            var property = entry.Property(auditableField.FieldName);
                            var currentValue = property.CurrentValue != null
                                ? property.CurrentValue.ToString()
                                : string.Empty;

                            var dbValue = (dbValues.GetDbPropertyValue(auditableField.FieldName) ?? String.Empty).ToString();
                            if (currentValue != dbValue)
                            {
                                fieldsChanged = true;

                                writer.WriteStartElement(auditableField.FieldName);
                                //writer.WriteElementString("action", "Modified");
                                writer.WriteElementString("description", currentValue);

                                writer.WriteEndElement();
                            }

                        }
                    }
                    else if (stateToBuild == EntityState.Added)
                    {
                        foreach (var auditableField in auditableFields)
                        {
                            var property = entry.Property(auditableField.FieldName);
                            var currentValue = property.CurrentValue != null
                                ? property.CurrentValue.ToString()
                                : string.Empty;

                            writer.WriteStartElement(property.Name);
                            //writer.WriteElementString("action", "Added");
                            writer.WriteElementString("description", currentValue);

                            writer.WriteEndElement();
                        }
                    }
                }
                writer.WriteEndElement();

            }
            return xml.ToString();
        }
        private string GetXmlReference(DbEntityEntry entry, string referenceCollectionName, string auditPropertyName, Type descriptionPropertyType, string descriptionPropertyName)
        {
            //por ahora sólo add y remove
            //TODO: ver cómo manejar la modificación
            if (entry.State != EntityState.Added && entry.State != EntityState.Deleted) return string.Empty;

            var xml = new StringBuilder();
            var settings = new XmlWriterSettings { Indent = true, IndentChars = "\t", NewLineOnAttributes = true };

            //obtengo la descripción para la auditoría
            var auditField = entry.Property(auditPropertyName).CurrentValue;
            string description;
            if (entry.Entity.GetType().IsAssignableFrom(descriptionPropertyType))
            {
                description = entry.Property(descriptionPropertyName).CurrentValue.ToString();
            }
            else
            {
                var descriptionEntityEntry = _dbContext.Entry(_dbContext.Set(descriptionPropertyType).Find(auditField));
                description = descriptionEntityEntry.Property(descriptionPropertyName).CurrentValue.ToString();
            }
            using (XmlWriter writer = XmlWriter.Create(xml, settings))
            {
                writer.WriteStartElement("changeSet");
                writer.WriteStartElement(referenceCollectionName);
                writer.WriteElementString("action", Enum.GetName(typeof(EntityState), entry.State));
                writer.WriteElementString(auditPropertyName, auditField.ToString());
                if (!string.IsNullOrEmpty(description))
                {
                    writer.WriteElementString("description", description);
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            return xml.ToString();
        }


    }
}