using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.PCP
{
    [Table("EstadosCargaMaquinas")]
    public class EstadosCargaMaquina
    {
        [Key]
        public int CG_ESTADO { get; set; }
        public string ESTADO { get; set; }
    }

}
