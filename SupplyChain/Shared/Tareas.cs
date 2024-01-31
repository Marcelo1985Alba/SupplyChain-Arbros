using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("tareas")]
    public class Tareas : EntityBase<int>
    {
        [Key, Column("Id")]
        new public int Id { get; set; }
        public String Titulo { get; set; }
        public String Estado { get; set; }
        public String Resumen { get; set; }
        public String Modulo { get; set; }
        [NotMapped]
        public List<string> Asignados { get; set; } = new List<string>();
    }
}
