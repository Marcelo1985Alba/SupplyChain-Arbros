using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.DTOs
{
    public class SolicitudDTO
    {
        public DateTime Fecha { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; } = 1;
        public int TagId { get; set; }
        public string Cuit { get; set; }
        public string Medidas { get; set; }
        public string Orifico { get; set; }
        public string SerieEntrada { get; set; }
        public string TipoEntrada { get; set; }
        public string SerieSalida { get; set; }
        public string TipoSalida { get; set; }
        public string Accesorios { get; set; }
        public string Asiento { get; set; }
        public string Bonete { get; set; }
        public string Cuerpo { get; set; }
        public string Resorte { get; set; }
        public string Disco { get; set; }
        public string Tobera { get; set; }
    }
}
