using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class Archivo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Directorio { get; set; }
        public string[] Contenido { get; set; }
        public byte[] ContenidoByte { get; set; }
        //public string ContenidoBase64 { get; set; }
    }
}
