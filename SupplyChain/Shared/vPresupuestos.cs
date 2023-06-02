using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vPresupuestos
    {
        [Key]
        [Display(Name ="PRESUPUESTOS")]
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int CG_CLI { get; set; } = 0;
        [Display(Name = "CLIENTE")]
        public string DES_CLI { get; set; }
        public string USUARIO { get; set; }
        public string MONEDA { get; set; }
        public decimal TOTAL { get; set; } = 0;
        public bool TIENEPEDIDO { get; set; }
        public int SEMAFORO => getSemaforo();
        public string COMENTARIO { get; set; }

        protected int getSemaforo()
        {
            int color = 0;

            if (SEMAFORO.ToString() == "Rojo")
                color = 1;
            else if (SEMAFORO.ToString() == "Amarilla")
                color = 2;
            else if (SEMAFORO.ToString() == "Verde")
                color = 3;
            return color;
        }
    }
}
