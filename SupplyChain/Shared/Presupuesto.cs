using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("PRESUPUESTO_ENCABEZADO")]
    public class Presupuesto : EntityBase<int>
    {
        
        public DateTime FECHA { get; set; } = DateTime.Now;
        [Required(ErrorMessage ="* La Moneda es requerida")]
        public string MONEDA { get; set; } = "DOLARES";
        [Required(ErrorMessage = "* El Cliente es requerido")]
        public int CG_CLI { get; set; } = 0;
        [Required(ErrorMessage = "* La Condicion de Pago es requerida")]
        public int CONDICION_PAGO { get; set; } = 0;
        [Required(ErrorMessage = "* La Condicion de Entrega es requerida")]
        public int CG_COND_ENTREGA { get; set; } = 0;
        public decimal BONIFIC { get; set; } = 0;
        public int CG_TRANS { get; set; } = 0;
        public double TC { get; set; } = 0;
        public decimal TOTAL { get; set; } = 0;

        public string USUARIO { get; set; } = "";

        ////[Required(ErrorMessage = "La Direccion de Entrega es requerida")]
        public string DIRENT { get; set; } = "";
        public bool TienePedido { get; set; }

        public string PROYECTO { get; set; } = "";
        public string INGENIERIA { get; set; } = "";
        public string REVISION { get; set; } = "";
        public string NRODOC { get; set; } = "";
        public string COMENTARIO { get; set; }
        public string COLOR { get; set; }

        [ValidateComplexType]
        public virtual List<PresupuestoDetalle> Items { get; set; } = new();

        [NotMapped]
        public string DES_CLI { get; set; } = "";

        [NotMapped]
        public bool ESNUEVO { get; set; }

        [NotMapped]
        public bool GUARDADO { get; set; }

    }
}
