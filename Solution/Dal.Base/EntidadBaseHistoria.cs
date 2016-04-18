using System;

namespace Dal.Base
{
    public class EntidadBaseHistoria:EntityBase
    {
        public DateTime VigenciaDesde { get; set; }
        public DateTime VigenciaHasta { get; set; }
    }
}