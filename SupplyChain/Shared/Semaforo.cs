using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("Semaforo")]
    public class Semaforo : EntityBase<int>
    {
        public string Color { get; set; }
        public int ID { get; set; }
      
    }
}
