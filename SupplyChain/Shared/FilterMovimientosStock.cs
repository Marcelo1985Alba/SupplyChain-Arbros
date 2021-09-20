using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class FilterMovimientosStock
    {
        public int Tipoo { get; set; } = 0;
        public int Deposito { get; set; } = 0;
        public string Desde { get; set; }
        public string Hasta { get; set; }
    }
}
