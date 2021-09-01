using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.PCP
{
    public class vProdMaquinaDataCore
    {
        [Key]
        public int Id { get; set; }
        public int Año { get; set; }
        public int Mes { get; set; }
        public string Maquina { get; set; }
        public decimal ParadasPlanHoras { get; set; }
        public decimal SetupRealHoras { get; set; }
        public decimal TiempoNetoHoras { get; set; }
    }
}
