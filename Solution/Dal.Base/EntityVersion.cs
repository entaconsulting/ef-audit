using System;

namespace Dal.Base
{
    public class EntityVersion:EntityBase
    {
        public DateTime VigenciaDesde { get; set; }
        public DateTime VigenciaHasta { get; set; }
    }
}