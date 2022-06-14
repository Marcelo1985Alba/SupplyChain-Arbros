using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain
{
    public class vEstadEventos
    {
        public DateTime FE_EMIT { get; set; }
        public DateTime FE_OCURRENCIA { get; set; }
        public int Cg_NoConf { get; set; }
        public string Des_TipoNc { get; set; }
        public string DES_PROVE { get; set; }
        public int MES { get; set; }
        public int ANIO { get; set; }
    }
}
