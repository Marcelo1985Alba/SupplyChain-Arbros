using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SupplyChain
{
    [Table("ProcalMP")]

    public class ProcalsMP : EntityBase<int>
    {
     //   List<ProcalsMP> list = new List<ProcalsMP>();
        
        [Key, Column("REGISTRO")]
        new public int Id { get; set; } = 0;
        public string CG_LINEA { get; set; } = "";
        public string DESCAL { get; set; } = "";
        public string CARCAL { get; set; } = "";
        public string UNIDADM { get; set; } = "";
        public string CANTMEDIDA { get; set; } = "";
        public decimal MEDIDA { get; set; } = 0;
        public string TOLE1 { get; set; } = "";
        public string TOLE2 { get; set; } = "";
        public string OBSERV { get; set; } = "";
        //public string FE_REG { get; set; } = "";
        public string DESCAL2 { get; set; } = "";
        public string CARCAL2 { get; set; } = "";
        public string OBSERV2 { get; set; } = "";
        public int PRIORIDAD { get; set; } = 0;

        [NotMapped] public bool GUARDADO { get; set; }
        [NotMapped] public bool ESNUEVO { get; set; }
    }

}
        