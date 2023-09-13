using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("VistasListados")]
    public class VistasGrillas
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string AppName { get; set; }
        public string Layout { get; set; }
        public string Usuario { get; set; }

    }
}
