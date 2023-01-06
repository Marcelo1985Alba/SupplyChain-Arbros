using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vControlCalidadPendientes
    {
        [Key]
        [Display (Name ="VALE")]
        public int Id { get; set; }
        public string DESPACHO { get; set; }
        public string CG_PROD { get; set; }
        public string CG_DEP { get; set; }
        public string CG_LINEA { get; set; }
        public string DESCAL { get; set; }
        public string CARCAL { get; set; }
        public string UNIDADM { get; set; }
        public string TOLE1 { get; set; }   
        public string TOLE2 { get; set;}
        public string AVISO { get; set; }
        public string CG_PROVE { get; set; }
        public string REMITO { get; set; }
        public string OCOMPRA { get; set; }

        [ValidateComplexType]
        public virtual List<CargaValoresDetalles> Items { get; set; } = new();

        [NotMapped]
        public bool ESNUEVO { get; set; }

        [NotMapped]
        public bool GUARDADO { get; set; }

    }
}
