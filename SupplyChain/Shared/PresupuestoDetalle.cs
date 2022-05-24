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
        public int PRESUPUESTOID { get; set; }
        public int SOLICITUDID { get; set; } = 0;
        [StringLength(maximumLength: 15, MinimumLength = 5)]
        public string CG_ART { get; set; }
        [Range(minimum:1, maximum:100, ErrorMessage ="La cantidad es requerida")]
        public decimal CANTIDAD { get; set; }
        public decimal PREC_UNIT{ get; set; }
        public decimal PREC_UNIT_X_CANTIDAD { get; set; }
        public decimal IMP_DESCUENTO { get; set; }
        public string OBSERITEM { get; set; } = "";
        public decimal DESCUENTO { get; set; }
        public int DIAS_PLAZO_ENTREGA { get; set; } = 0;
        public string MEDIDAS { get; set; } = "";
        public string ORIFICIO { get; set; } = "";
        public string SERIEENTRADA { get; set; } = "";
        public string TIPOENTRADA { get; set; } = "";
        public string SERIESALIDA { get; set; } = "";
        public string TIPOSALIDA { get; set; } = "";
        public string ACCESORIOS { get; set; } = "";
        public string ASIENTO { get; set; } = "";
        public string BONETE { get; set; } = "";
        public string CUERPO { get; set; } = "";
        public string RESORTE { get; set; } = "";
        public string DISCO { get; set; } = "";
        public string TOBERA { get; set; } = "";
        public Presupuesto Presupuesto { get; set; }
        public Solicitud Solicitud { get; set; }

        [NotMapped]
        public string  DES_ART { get; set; }

    }
}
