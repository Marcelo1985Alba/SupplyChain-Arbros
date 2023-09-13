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
    [Table("Modulo_Usuario")]
    public class ModulosUsuario : EntityBase<int>
    {
        public int ModuloId { get; set; }
        public string UserId { get; set; }
    }
}
