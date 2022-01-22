using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vEstadPedidosIngresados : EntityBase
    {
        public int PEDIDO { get; set; }
        public DateTime FECHA { get; set; }
        public string CG_ART { get; set; }
        public string DES_ART { get; set; }
        public decimal CANTPED { get; set; }
        public decimal UNIDEQUI { get; set; }
        public int MES { get; set; }
        public int ANIO { get; set; }
    }
}
