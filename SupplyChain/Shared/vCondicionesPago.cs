using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vCondicionesPago : EntityBase<int>
    {
        public string DESCRIPCION { get; set; }
        public int DIAS { get; set; }
        public bool CONTADO { get; set; }
    }
}
