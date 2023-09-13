using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.PCP
{
    public class vResumenStock
    {
        [Display(Name = "Codigo Articulo")]
        public string CG_ART { get; set; }
        [Display(Name = "Despacho")]
        public string DESPACHO { get; set; } = "";
        [Display(Name = "Lote")]
        public string LOTE { get; set; } = "";
        [Display(Name = "Serie")]
        public string SERIE { get; set; } = "";
        [Display(Name = "Ubicacion")]
        public string UBICACION { get; set; } = "";
        [Display(Name = "Stock")]
        public decimal STOCK { get; set; } = 0;
        [Display(Name = "Deposito")]
        public int CG_DEP { get; set; } = 0;
        [Display(Name = "Orden Ingreso")]
        public int CG_ORDING { get; set; } = 0;
        [Display(Name = "Descripcion Deposito")]
        public string DEPOSITO { get; set; }
        [Display(Name = "Descripcion Articulo")]
        public string PRODUCTO { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Display(Name ="Id")]
        public int Registro { get; set; }
    }
}
