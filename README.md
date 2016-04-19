# ef-extensions
Este proyecto consolida un conjunto de librerías y herramientas que implementan herramientas y patrones de uso frecuente sobre la capa de acceso a datos (DAL) de proyectos desarrollados sobre Entity Framework.
Se trata de código desarrollado por Enta Consulting durante sus proyectos de desarrollo sobre tecnología .Net

Dal.Base
--------
  
Implementa los patrones Unit of Work y Repository sobre un DbContext de EF, y define las superclases que deben ser heredadas por las entidades de dominio para funcionar con esta librería.

Ver [RepositoryTest.cs](https://github.com/entaconsulting/ef-extensions/blob/master/Solution/Dal.Test/RepositoryTest.cs) para ejemplos de las operaciones de Repository 
  
Dal.History
-----------
  
Agrega facilidades para manejar versionado de atributos en entidades.
El versionado de atributos se implementa en clases que deben heredar de **EntityVersionSetItem**, generando una relación 1:N entre la entidad principal y la entidad que define los atributos que deben versionarse:

    //esta es la entidad real
    public class Foo
    {
       ...
       public ICollection<FooVersion> VersionAttributes {get; set;}
    }
    //esta entidad implementa la historicidad para los atributos 1 y 2
    public class FooVersion : EntityVersionSetItem
    {
      public string Attribute1 {get; set;}
      public int Attribute2 {get; set;}
    }


Implementa operaciones tales como

    //obtener la versión vigente a un momento de los atributos de la entidad foo
    var effective = foo.GetVersion<Foo, FooVersion>(pointInTime, unitOfWork)

    //generar una nueva versión de datos a partir de un momento
    var newVersion = foo.CreateVersion(x=>x.VersionAttributes, pointInTime, unitOfWork)

En [HistoryTest.cs](https://github.com/entaconsulting/ef-extensions/blob/master/Solution/Dal.Test/HistoryTest.cs) se describen más ejemplos de uso de la librería y aplicación a casos concretos.

Dal.Audit
---------

Implementa auditoría de datos en forma no obtrusiva sobre DbContext.

El módulo se compone de 4 elementos:
* AuditProfile

  Configuración a través de fluent api las entidades a auditar y el nivel de detalle de cada una
  
* AuditProvider
  Implementación el formato de persistencia de la auditoría. 
  A través de este elemento se pueden implementar diferentes formas de persistencia de auditoría, por ejemplo en base de datos, en file system, etc.
  Actualmente existe una implementación que registra la auditoría en base de datos SQL Server, en una tabla, utilizando formato XML para registrar los detalles a nivel campo.
* Application Context
  Implementación de métodos para obtener nombre de usuario y otros datos de context que se guardarán en la auditoría.
* AuditManager
  Es el elemento que concentra la configuración del módulo y expone métodos para facilitar la obtención del historial de cambios de entidades


##### Ejemplo AuditProfile:

    public class AuditSampleProfile : AuditProfile
    {
        public override void Configure()
        {
            base.Configure();

            //auditar todas las entidades que heredan de BaseEntity sólo a nivel entidad, sin detalle por campos
            AuditAllOfType<EntityBase>(e => e.Id)
                .IgnoreIfNoFieldChanged();

            //en el caso de usuario, auditar a nivel campo cambios en Nombre y Fecha de nacimiento
            AddAuditable<Usuario>(e => e.Id)
                .IgnoreIfNoFieldChanged()
                .AuditField(e => e.Nombre)
                .AuditField(e => e.FechaNacimiento);
        }
    }

##### Ejemplo inicialización de auditoría sobre un DbContext
      //Crear una instancia de AuditManager con los 3 elementos de configuración
      _auditManager = new AuditManager(auditProvider,auditProfile,appContext);
      
      //Activar la auditoría sobre un dbContext
      var auditableDbContext = dbContext.BeginAudit(_auditManager);

##### Ejemplo de obtención de historia de cambios en un campo de una entidad
      //AuditManager facilita las consultas sobre el log de auditoría
      //En este caso se obtienen todos los momentos de modificación del campo "Nombre", ignorando el resto de los cambios
      //si es que no se modificó dicho campo
      
      var nameChangeHistory = _auditManager.GetFieldHistory(usuario, x=>x.Nombre);

Ver [AuditTest.cs](https://github.com/entaconsulting/ef-extensions/blob/master/Solution/Dal.Test/AuditTest.cs) para ejemplos de casos de auditoría y lectura de historial



