using SupplyChain.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SupplyChain
{
    [Table("ProcalMP")]
    public class ProcalsMP : EntityBase<int>
    {
        [Key, Column("CG_LINEA")]
        new public int Id { get; set; } = 0;
        public string DESCAL { get; set; } = "";
        public string CARCAL { get; set; } = "";
        public string UNIDADM { get; set; } = "";
        public string CANTMEDIDA { get; set; } = "";
        public string MEDIDA { get; set; } = "";
        public string TOLE1 { get; set; } = "";
        public string TOLE2 { get; set; } = "";
        public string OBSERV { get; set; } = "";
        //public string FE_REG { get; set; } = "";
        public string DESCAL2 { get; set; } = "";
        public string CARCAL2 { get; set; } = "";
        public string OBSERV2 { get; set; } = "";
        public string PRIORIDAD { get; set; } = "";

        [NotMapped] public bool GUARDADO { get; set; }
        [NotMapped] public bool ESNUEVO { get; set; }
    }

}
