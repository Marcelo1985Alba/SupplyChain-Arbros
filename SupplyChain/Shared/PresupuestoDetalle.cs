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

        public string MEDIDAS { get; set; }
        public string ORIFICIO { get; set; }
        public string SERIEENTRADA { get; set; }
        public string TIPOENTRADA { get; set; }
        public string SERIESALIDA { get; set; }
        public string TIPOSALIDA { get; set; }
        public string ACCESORIOS { get; set; }
        public string ASIENTO { get; set; }
        public string BONETE { get; set; }
        public string CUERPO { get; set; }
        public string RESORTE { get; set; }
        public string DISCO { get; set; }
        public string TOBERA
        {
            get; set;
        }
        public Presupuesto Presupuesto { get; set; }

        [NotMapped]
        public string  DES_ART { get; set; }

    }
}
