using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vDireccionesEntrega : EntityBase<int>
    {
        public string ID_CLIENTE { get; set; }
        public string DESCRIPCION { get; set; }
    }
}
