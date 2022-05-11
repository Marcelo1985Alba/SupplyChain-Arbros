using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("PRESUPUESTO_DETALLE")]
    public class PresupuestoDetalle : EntityBase<int>
    {
        public int PRESUPUESTOENCABEZADOID { get; set; }
        [StringLength(maximumLength: 15, MinimumLength = 15)]
        public string CG_ART { get; set; }
        public decimal CANTIDAD { get; set; }
        public decimal PREC_UNIT_X_CANTIDAD { get; set; }
        public decimal IMP_DESCUENTO { get; set; }
        public string OBSERITEM { get; set; }
        public decimal DESCUENTO { get; set; }
        public Presupuesto Presupuesto { get; set; }


    }
}
