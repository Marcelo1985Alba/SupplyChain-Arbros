using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("Semaforo")]
    public class Semaforo
    {
        public int ID { get; set; }
        public string COLOR { get; set; }
    }
}
