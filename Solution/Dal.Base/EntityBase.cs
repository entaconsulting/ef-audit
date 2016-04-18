using System.ComponentModel.DataAnnotations;

namespace Dal.Base
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
