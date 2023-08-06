using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vIngenieriaProductosFormulas
    {
        public string CG_PROD { get; set; }
        public string DES_PROD { get; set; }
        public bool TIENE_FORM { get; set; }
        public bool FORM_ACTIVA { get; set; }
        public decimal PRECIO { get; set; }
        [NotMapped] public decimal CMP { get; set; } // Costo de materia prima
        [NotMapped] public decimal CGGF { get; set; } // Costo de Gastos Grales de Fabricación
        [NotMapped] public decimal CT => CMP + CGGF;
        [NotMapped] public decimal UT => PRECIO - CT;
    }
}
