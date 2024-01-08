using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("ProcunProcesos")]
    public class ProcunProcesos 
    {
        public string PROCESO {  get; set; }
        //public int ID { get; set; } = 0;
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int REGISTRO { get; set; } = 0;
    }
}
