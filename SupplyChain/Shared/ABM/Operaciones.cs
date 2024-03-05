using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SupplyChain.Shared.Models
{
    [Table("Procesos")]
    public class Operaciones : EntityBase<int>
    {
        [Key, Column("REGISTRO")]
        public new int Id { get; set; }

        public string VALE { get; set; } = "";
        public string DESPACHO { get; set; } = "";
        public DateTime FE_ENSAYO { get; set; }
        public string CG_PROD { get; set; } = "";
        public int CG_ORDEN { get; set; }
        public string DESCAL { get; set; } = "";
        public string CARCAL { get; set; } = "";
        public string UNIDADM { get; set; } = "";
        public int CANTMEDIDA { get; set; }
        public decimal MEDIDA { get; set; }
        public decimal TOLE1 { get; set; }
        public decimal TOLE2 { get; set; }
        public string? OBSERV { get; set; } = "";
        public string? AVISO { get; set; } = "";
        public decimal MEDIDA1 { get; set; }
        public string? OBSERV1 { get; set; } = "";
        public int CG_PROVE { get; set; }
        public string? REMITO { get; set; } = "";
        public string? VALORNC { get; set; } = "";
        public string? LEYENDANC { get; set; } = "";
        public string? LOTE { get; set; } = "";
        public int CG_ORDF { get; set; }
        public string? UNID { get; set; } = "";
        public int NUM_PASE { get; set; }
        public string? ENSAYOS { get; set; } = "";
        public DateTime FE_REG { get; set; }
        public string APROBADO { get; set; } = "";
        public string? TIPO { get; set; } = "";
        public Boolean ANULADO { get; set; }
        public int CG_CLI { get; set; }
        public string? Usuario { get; set; } = "";

        [NotMapped]
        public bool ESNUEVO { get; set; }
        [NotMapped]
        public bool GUARDADO { get; set; }
    }
}