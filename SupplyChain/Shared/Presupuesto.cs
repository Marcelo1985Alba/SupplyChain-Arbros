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

        public int SOLICITUD { get; set; }
        public int DIAS_PLAZO_ENTREGA { get; set; } = 0;
        public DateTime FECHA { get; set; } = DateTime.Now;
        [Required]
        public string MONEDA { get; set; }
        public decimal IVA { get; set; }
        public int CG_CLI { get; set; } = 0;
        public int CONDICION_PAGO { get; set; } = 0;
        public decimal BONIFIC { get; set; }
        public int CG_TRANS { get; set; } = 0;
        public int CG_EXPRESO { get; set; } = 0;

        [Required]
        public string DIRENT { get; set; }
        [NotMapped]
        public string DES_ART { get; set; }
        [NotMapped]
        public string UNID { get; set; }

        [NotMapped]
        public string DES_CLI { get; set; }

        [NotMapped]
        public bool ESNUEVO { get; set; }

        [NotMapped]
        public bool GUARDADO { get; set; }
        public virtual List<PresupuestoDetalle> Items { get; set; } = new();
    }
}
