using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vEstadPedidosAlta : EntityBase
    {
        public int PEDIDO { get; set; }
        public DateTime FECHA { get; set; }
        public string CG_ART { get; set; }
        public string DES_ART { get; set; }
        public decimal STOCK { get; set; }
        public decimal UNIDEQUI { get; set; }
        public int MES { get; set; }
        public int ANIO { get; set; }
    }
}
