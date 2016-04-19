# ef-extensions
Este proyecto consolida un conjunto de librerías y herramientas que implementan herramientas y patrones de uso frecuente sobre la capa de acceso a datos (DAL) de proyectos desarrollados sobre Entity Framework.
Se trata de código desarrollado por Enta Consulting durante sus proyectos de desarrollo sobre tecnología .Net

Dal.Base
--------
  
Implementa los patrones Unit of Work y Repository sobre un DbContext de EF, y define las superclases que deben ser heredadas por las entidades de dominio para funcionar con esta librería.
  
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

