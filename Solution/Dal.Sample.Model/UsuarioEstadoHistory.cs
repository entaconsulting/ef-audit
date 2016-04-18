using System.ComponentModel.DataAnnotations.Schema;
using Dal.Base;

namespace Dal.Sample.Model
{
    public class UsuarioEstadoHistory:EntidadBaseHistoriaAtributos
    {
        public bool Habilitado { get; set; }
    }
}