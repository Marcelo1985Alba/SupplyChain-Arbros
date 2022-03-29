using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vEstadoPedido : EntityBase
    {
        public int PEDIDO { get; set; }
        public DateTime FE_PED { get; set; }
        public int CG_CLI { get; set; }
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
    }
}
