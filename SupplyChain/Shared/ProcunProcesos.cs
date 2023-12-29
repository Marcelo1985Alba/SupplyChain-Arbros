using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("ProcunProcesos")]
    public class ProcunProcesos : EntityBase<int>
    {
        public string PROCESO {  get; set; }
        //public int ID {  get; set; }
        public int REGISTRO {  get; set; }
    }
}
