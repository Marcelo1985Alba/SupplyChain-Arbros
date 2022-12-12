/*using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.PCP
{
    [Table("CargaProcesos")]
    public class CargaProcesos : EntityBase<int>
    {
        [Key, Column("REGISTRO")]

        new public int Id { get; set; } = 0;
        public string VALE { get; set; } = "";
        public int CERTIF { get; set; } = 0;
        public DateTime FE_ENSAYO { get; set; } = DateTime.Today;
        public string CG_PROD { get; set; } = "";
        public string DESCAL { get; set; } = "";
        public string CARCAL { get; set; } = "";
        public string UNIDADM { get; set; } = "";
        public int CANTMEDIDA { get; set; } = 0;
        public string OBSERV { get; set; } = "";
        public string AVISO { get; set; } = "";
        public string OBSERV1 { get; set; } = "";
        public int CG_PROVE { get; set; } = 0;
        public string REMITO { get; set; } = "";
        public string VALORNC { get; set; } = "";
        public string LEYENDANC{ get; set; } = "";
        public int O_COMPRA{ get; set; } = 0;
        public string UNID { get; set; } = "";
        public int EVENTO { get; set; } = 0;
        public string ENSAYO { get; set; } = "";
        public DateTime FECHA { get; set; } = DateTime.Today;
        public string APROBADO { get; set; } = "";
        public string Usuario { get; set; } = "";
        public int REGISTRO { get; set; } = 0;

        [NotMapped] public bool GUARDADO { get; set; }
        [NotMapped] public bool ESNUEVO { get; set; }

    }
}
*/