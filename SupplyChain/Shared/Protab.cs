using Microsoft.EntityFrameworkCore;
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
    [Table("PROTAB")]
    public class Protab : EntityBase<string>
    {
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column("PROCESO")]
        public new string Id { get; set; } ="";
        public string DESCRIP { get; set; } = "";
        //public int ID { get; set; } = 0;
        //public string PROCESO { get; set; }= "";
        [NotMapped]
        public bool ESNUEVO { get; set; }
        [NotMapped]
        public bool GUARDADO { get; set; }
        
    }
}
