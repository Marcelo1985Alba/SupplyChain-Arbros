using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#nullable enable
namespace SupplyChain.Shared
{
    [Table("Modulos")]
    public class Modulo : EntityBase<int>
    {
        public string Descripcion { get; set; }
        public int? ParentId { get; set; }
        public bool TieneChild { get; set; }
        public string? Url { get; set; }
        public string? IconCss { get; set; }
    }
}
