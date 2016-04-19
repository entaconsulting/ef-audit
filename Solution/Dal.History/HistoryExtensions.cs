using Dal.History.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using Dal.Base;
using DAL;

namespace Dal.History
{
    public static class HistoryExtensions
    {
        public static readonly DateTime MaxDate = new DateTime(2100, 12, 31);

        public static IEnumerable<T> EffectiveAt<T>(this IEnumerable<T> query, DateTime fechaVersion) where T : EntityVersion
        {
            return query.EffectiveBetween(fechaVersion, fechaVersion);
        }
        public static IQueryable<T> EffectiveAt<T>(this IQueryable<T> query, DateTime fechaVersion) where T : EntityVersion
        {
            return query.EffectiveBetween(fechaVersion, fechaVersion);
        }
        public static IEnumerable<T> EffectiveBetween<T>(this IEnumerable<T> query, DateTime fechaDesde, DateTime fechaHasta) where T : EntityVersion
        {
            return query.Where(r => r.VigenciaDesde <= fechaHasta && r.VigenciaHasta >= fechaDesde);
        }
        public static IQueryable<T> EffectiveBetween<T>(this IQueryable<T> query, DateTime fechaDesde, DateTime fechaHasta) where T : EntityVersion
        {
            return query.Where(r => r.VigenciaDesde <= fechaHasta && r.VigenciaHasta >= fechaDesde);
        }

        /// <summary>
        /// Hace un join del mismo query pero con el registro anterior en la serie historica
        /// Sirve para cuando hay que hacer comparaciones entre los historicos para detectar cambios en algunos campos por ejemplo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condicion"></param>
        /// <returns></returns>
        public static IQueryable<T> CompareWithPrevious<T>(this IQueryable<T> query, Expression<Func<VersionTuple<T>,bool>> condicion ) where T : EntityVersionSetItem
        {
            //agrego a la condicion el hecho de que la anterior puede no existir en cuyo caso considero a la actual como que es un cambio (porque es la primera)
            Expression<Func<VersionTuple<T>,bool>> condEsNull = (t) => t.Referido == null;
            var condCompuesta = condicion.OrElse(condEsNull);

            var result = query
                .GroupJoin(query
                    , actual => new {IdPadre = actual.VersionSetId, Fecha = actual.VigenciaDesde}
                    , anterior =>
                        new {IdPadre = anterior.VersionSetId, Fecha = SqlFunctions.DateAdd("d", 1, anterior.VigenciaHasta).Value}
                    , (actual, anteriores) =>
                        new VersionTuple<T> {Actual = actual, Referido = anteriores.FirstOrDefault()})
                .Where(condCompuesta)
                .Select(t => t.Actual);
            
            return result;
        }


        public class VersionTuple<T>
        {
            public T Actual { get; set; }
            public T Referido { get; set; }
        }

        public static TV GetVersion<T, TV>(this T entidad, DateTime fechaVersion, IUoW uowAUsar, params Expression<Func<TV, object>>[] includes)
            where T : EntityBase
            where TV : EntityVersionSetItem
        {
            return GetVersion(entidad.Id, fechaVersion, uowAUsar, includes).FirstOrDefault();
        }

        public static TV GetDefaultVersion<T, TV>(this T entidad, DateTime fechaVersion)
            where T : EntityBase
            where TV : EntityVersionSetItem, new()
        {
            return new TV()
            {
                VigenciaDesde = fechaVersion,
                VigenciaHasta = new DateTime(2100,12,31),
                VersionSetId = entidad.Id
            };
        }



        public static TV GetDefaultVersion<TV>(this int entidadId, DateTime fechaVersion)
            where TV : EntityVersionSetItem, new()
        {
            return new TV()
            {
                VigenciaDesde = fechaVersion,
                VigenciaHasta = MaxDate,
                VersionSetId = entidadId
            };
        }


        public static IQueryable<TV> GetVersion<TV>(this int entidadId, DateTime fechaVersion, IUoW uowAUsar, params Expression<Func<TV, object>>[] includes)
            where TV : EntityVersionSetItem
        {
            var query = uowAUsar.GetRepository<TV>().Query().Where(ev => ev.VersionSetId == entidadId)
                .EffectiveAt(fechaVersion);

            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            return query;

        }




        public static IQueryable<TV> GetVersion<T, TV>(this IEnumerable<T> entidades, DateTime fechaVersion, IUoW uowAUsar, params Expression<Func<TV, object>>[] includes)
            where T : EntityBase
            where TV : EntityVersionSetItem
        {
            var idsPadre = entidades.Select(e => e.Id);
            return idsPadre.GetVersion(fechaVersion, uowAUsar, includes);
        }

        public static IQueryable<TV> GetVersion<TV>(this IEnumerable<int> entidadesIds, DateTime fechaVersion, IUoW uowAUsar, params Expression<Func<TV, object>>[] includes)
            where TV : EntityVersionSetItem
        {

            var query = uowAUsar.GetRepository<TV>().Query().EffectiveAt(fechaVersion)
                .Where(ev => entidadesIds.Contains(ev.VersionSetId));


            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }

        public static TV CreateVersion<TV>(this int entidadId,DateTime fecha, IUoW uowAUsar, TV objetoHistoriaBase = null, bool forzarNuevaEntidad=false, bool agregarAlRepositorio = true)where TV : EntityVersionSetItem, new()
        {
            var repositorioVersion = uowAUsar.GetRepository<TV>();
            //obtengo la versión vigente
            var versionVigente = entidadId.GetVersion<TV>(fecha, uowAUsar).FirstOrDefault();
            if (versionVigente != null && versionVigente.VigenciaDesde == fecha && !forzarNuevaEntidad)
                //si la fecha desde es igual que la versión vigente no se genera una nueva sino que se trabaja sobre la misma
                return versionVigente;

            //genero la nueva versión
            var versionNueva = objetoHistoriaBase ?? new TV() {VersionSetId = entidadId};
            versionNueva.VigenciaDesde = fecha;

            //actualizo campos de versionado
            if (versionVigente != null)
            {
                versionNueva.VigenciaHasta = versionVigente.VigenciaHasta;

                versionVigente.VigenciaHasta = fecha.AddDays(-1);
                repositorioVersion.Update(versionVigente);

            }
            else
            {
                //busco si tengo una versión posterior para tomar la vigencia hasta, si no es el maximo
                var versionPosterior = repositorioVersion.Query()
                    .Where(v =>  v.VersionSetId == entidadId && v.VigenciaDesde > fecha)
                    .OrderBy(v => v.VigenciaDesde)
                    .FirstOrDefault();
                versionNueva.VigenciaHasta = versionPosterior != null
                    ? versionPosterior.VigenciaDesde.AddDays(-1)
                    : MaxDate;
            }

            versionNueva.VersionSetId = entidadId;

            if (agregarAlRepositorio)
            {
                repositorioVersion.Add(versionNueva);
            }

            return versionNueva;

        }
        public static TV CreateVersion<T, TV>(this T entidad, Expression<Func<T, ICollection<TV>>> propiedadVersion, DateTime fecha, IUoW uowAUsar)
            where T : EntityBase
            where TV : EntityVersionSetItem, new()
        {
            var versionNueva = entidad.Id.CreateVersion<TV>(fecha, uowAUsar);
            return versionNueva;
        }
        public static T ReplaceVersion<T>(this T versionVigente, DateTime fecha, IUoW uowAUsar)
            where T : EntityVersion, new()
        {

            var repositorio = uowAUsar.GetRepository<T>();

            if (versionVigente.VigenciaDesde == fecha)
            {
                //si la fecha desde es igual que la versión vigente no se genera una nueva sino que se trabaja sobre la misma
                return versionVigente;
            }

            //genero la nueva versión
            var versionNueva = new T { VigenciaDesde = fecha, VigenciaHasta = versionVigente.VigenciaHasta };

            //actualizo campos de versionado
            versionVigente.VigenciaHasta = fecha.AddDays(-1);

            repositorio.Update(versionVigente);
            repositorio.Add(versionNueva);

            return versionNueva;
        }
        public static void DeleteVersion<T>(this T versionVigente, IUoW uowAUsar)
            where T : EntityVersionSetItem, new()
        {

            var repositorio = uowAUsar.GetRepository<T>();

            //busco las versiones anterior y posterior a la vigente
            var versionAnterior = repositorio.Query().Where(v => v.VersionSetId == versionVigente.VersionSetId && v.VigenciaHasta < versionVigente.VigenciaDesde)
                .OrderByDescending(v => v.VigenciaHasta)
                .FirstOrDefault();
            var versionPosterior = repositorio.Query().Where(v => v.VersionSetId == versionVigente.VersionSetId && v.VigenciaDesde > versionVigente.VigenciaHasta)
                .OrderBy(v => v.VigenciaDesde)
                .FirstOrDefault();

            //extiendo la versión anterior hasta el inicio de la versión posterior
            if (versionAnterior != null)
            {
                versionAnterior.VigenciaHasta = versionPosterior != null
                    ? versionPosterior.VigenciaDesde.AddDays(-1)
                    : MaxDate;

                repositorio.Update(versionAnterior);
            }

            //elimino la version vigente
            repositorio.Delete(versionVigente);

        }
        public static void UpdateEffectiveTo<T>(this T versionVigente, DateTime fecha, IUoW uowAUsar)
            where T : EntityVersion, new()
        {

            var repositorio = uowAUsar.GetRepository<T>();

            //actualizo la fecha hasta
            versionVigente.VigenciaHasta = fecha.AddDays(-1);

            repositorio.Update(versionVigente);
        }

        public static T GetVersionDefaultBase<T>(this T entidad, DateTime fechaVersion)
            where T : EntityVersion, new()
        {
            return new T()
            {
                VigenciaDesde = fechaVersion,
                VigenciaHasta = MaxDate,
                Id = entidad.Id
            };
        }
    }


}
