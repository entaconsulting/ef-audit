using System.ComponentModel.DataAnnotations.Schema;
using Dal.Base;

namespace Dal.Sample.Model
{
    public class UsuarioEstadoHistory:EntityVersionSetItem
    {
        public bool Habilitado { get; set; }
    }
}