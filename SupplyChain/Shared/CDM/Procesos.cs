using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SupplyChain
{
    [Table("Procesos")]
    public class Procesos : EntityBase<int>
    {
        [Key, Column("REGISTRO")]
        new public int Id { get; set; }
        public string VALE { get; set; } = "";
        public string DESPACHO { get; set; } = "";
        public DateTime? FE_ENSAYO { get; set; }
        public string CG_PROD { get; set; } = "";
        public int CG_ORDEN { get; set; } = 0;
        public string DESCAL { get; set; } = "";
        public string CARCAL { get; set; } = "";
        public string UNIDADM { get; set; } = "";
        public int CANTMEDIDA { get; set; } = 0;
        public decimal MEDIDA { get; set; } = 0;
        public decimal TOLE1 { get; set; } = 0;
        public decimal TOLE2 { get; set; } = 0;
        public string OBSERV { get; set; } = "";
        public string AVISO { get; set; } = "";
        public decimal MEDIDA1 { get; set; } = 0;
        public string OBSERV1 { get; set; } = "";
        public int CG_PROVE { get; set; } = 0;
        public string REMITO { get; set; } = "";
        public string VALORNC { get; set; } = "";
        public string LEYENDANC { get; set; } = "";
        public string LOTE { get; set; } = "";
        public int CG_ORDF { get; set; } = 0;
        public string UNID { get; set; } = "";
        public int NUM_PASE { get; set; } = 0;
        public string ENSAYOS { get; set; } = "";
        public DateTime? FECHA { get; set; }
        public string APROBADO { get; set; } = "";
        public string TIPO { get; set; } = "";
        public int CG_CLI { get; set; } = 0;
        public string USUARIO { get; set; } = "";
        public DateTime? FE_REG { get; set; } = DateTime.Now;

        [ValidateComplexType]
        public virtual List<CargaValoresDetalles> Items { get; set; } = new();
        [NotMapped] public bool GUARDADO { get; set; }
        [NotMapped] public bool ESNUEVO { get; set; }
    }   
}
