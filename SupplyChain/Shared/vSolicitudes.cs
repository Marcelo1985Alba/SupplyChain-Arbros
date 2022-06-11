using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vSolicitudes
    {

        [Key, Display(Name ="Solicitud Id")]
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        [StringLength(maximumLength: 15, MinimumLength = 15)]
        public string Producto { get; set; }

        [Display(Name = "Descripcion Producto")]
        public string Descripcion { get; set; }
        public int CG_CLI { get; set; } = 0;
        [Display(Name = "Descripcion Cliente")]
        public string DES_CLI { get; set; }
        public int CalcId { get; set; } = 0;
        public string Cuit { get; set; } = "";
        public int Cantidad { get; set; }
        public bool TienePresupuesto { get; set; }
    }
}
