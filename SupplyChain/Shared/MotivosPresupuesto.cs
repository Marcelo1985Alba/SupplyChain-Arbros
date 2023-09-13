using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("PRESUPUESTO_MOTIVOS")]
    public class MotivosPresupuesto : EntityBase<int>
    {
        //public int Id { get; set; }
        public string Motivo { get; set; } 

    }
}
