using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vTipoCambio : EntityBase<int>
    {
        public int Id_Moneda { get; set; }
        public string Descripcion { get; set; }
        public string Simbolo { get; set; }
        public DateTime Fecha_Cotiz { get; set; }
        public double Cotizacion { get; set; }
    }
}
