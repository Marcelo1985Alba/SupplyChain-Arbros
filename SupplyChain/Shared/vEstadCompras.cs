using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vEstadCompras : EntityBase
    {
        public DateTime FECHA { get; set; }
        public string REMITO { get; set; }
        public string LEYENDA { get; set; }
        public string FACTURA { get; set; }
        public string ARTICULO { get; set; }
        public string TIPO { get; set; }
        public decimal CANTIDAD { get; set; }
        public double TOTAL_DOL { get; set; }
        public string CUIT { get; set; }
        public string PROVEEDOR { get; set; }
        public int MES { get; set; }
        public int ANIO { get; set; }
    }
}
