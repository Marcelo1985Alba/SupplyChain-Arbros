using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class ConfigEmailCompras
    {
        public string ServidorSmtp { get; set; }
        public int Puerto { get; set; }
        public bool RequiereAutenticacion { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public string To { get; set; }
        public List<string> Copia { get; set; }
    }
}
