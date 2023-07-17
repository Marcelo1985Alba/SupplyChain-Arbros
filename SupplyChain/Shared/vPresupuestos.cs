using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vPresupuestos
    {
        [Key]
        [Display(Name ="PRESUPUESTOS")]
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int CG_CLI { get; set; } = 0;
        [Display(Name = "CLIENTE")]
        public string DES_CLI { get; set; }
        public string USUARIO { get; set; }
        public string MONEDA { get; set; }
        public decimal TOTAL { get; set; } = 0;
        public bool TIENEPEDIDO { get; set; }
        public string COLOR { get; set; }
        public int? ASIGNA { get; set; } = 0;
        public string COMENTARIO { get; set; } = "";
        public string MOTIVO { get; set; }

    }
}
