using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("tareasPorUsuario")]
    public class TareasPorUsuario : EntityBase<int>
    {
        [Key, Column("Id")]
        new public int Id { get; set; }
        public String userId { get; set; }
        public int tareaId { get; set; }
    }
}
