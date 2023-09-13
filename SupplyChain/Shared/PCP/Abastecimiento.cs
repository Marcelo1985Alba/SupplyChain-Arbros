using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain.Shared.Models
{
    [Table("NET_Temp_Abastecimiento")]
    public class ModeloAbastecimiento
    {
        public string CG_PROD { get; set; } = "";
        [Key]
        public string CG_MAT { get; set; } = "";
        public string DES_MAT { get; set; } = "";
        public int CG_ORDEN { get; set; } = 0;
        public decimal? CALCULADO { get; set; } = 0;
        public decimal? ACOMPRAR { get; set; } = 0;
        public decimal? ACOMPRAR_INFORMADO { get; set; } = 0;
        public decimal? STOCK { get; set; } = 0;
        public string UNIDMED { get; set; } = "";
        public string UNIDCOMER { get; set; } = "";
        public decimal? STOCK_MINIMO { get; set; } = 0;
        public decimal? PEND_SIN_OC { get; set; } = 0;
        public decimal? COMP_DE_ENTRADA { get; set; } = 0;
        public decimal? COMP_DE_SALIDA { get; set; } = 0;
        public decimal? STOCK_CORREG { get; set; } = 0;
        public decimal? EN_PROCESO { get; set; } = 0;
        public decimal? REQUERIDO { get; set; } = 0;
        public DateTime ENTRPREV { get; set; }

        [NotMapped]
        public int CantProcesos { get; set; } = 0;
        //public int CG_CIA { get; set; } = 0;
        //public string USUARIO { get; set; } = "";
    }

}
