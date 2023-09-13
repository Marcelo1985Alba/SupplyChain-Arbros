using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("Semaforo")]
    public class Semaforo : EntityBase<int>
    {
        //public int ID { get; set; }
        public string COLOR { get; set; }
        public int? ASIGNA { get; set; } = 0;
    }


}
