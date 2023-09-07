using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vEstadoPedido : EntityBase<int>
    {
        public int PEDIDO { get; set; }
        public DateTime FE_PED { get; set; }
        public int CG_CLI { get; set; }
        public int NUM_OCI { get; set; }
        public string DES_CLI { get; set; }
        public string ORCO { get; set; }
        public int ESTADO_PEDIDO { get; set; }
        public string DESCRIPCION_ESTADO_PEDIDO { get; set; }
        public string CG_ART { get; set; }
        public string DES_ART { get; set; }
        public decimal CANTPED { get; set; }
        public string UNID { get; set; }
        [Column("ENTRPREV")]
        public DateTime FE_PREV { get; set; }
        public string OBSERITEM { get; set; }
        public string? REMITO { get; set; }
        public string? LETRA_FACTURA { get; set; }
        public string? FACTURA { get; set; }
        public string? RECIBO { get; set; }
        public string? CONDICION_PAGO { get; set; }
        public double TOT_DOL { get; set; }

        public DateTime? FE_PLAN { get; set; }
        public DateTime? FE_FIRME { get; set; }
        public DateTime? FE_CIERRE { get; set; }
        public DateTime? FE_REM { get; set; }
        public DateTime? FE_FACT { get; set; }
        public DateTime? FE_RECIBO { get; set; }

    }
}
