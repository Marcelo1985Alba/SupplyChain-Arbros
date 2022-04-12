using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key, Display(Name ="Codigo")]
        public int CG_CLI { get; set; } = 0;
        [Display(Name = "Dewcripcion")]
        public string DES_CLI { get; set; } = "";
        public string CUIT { get; set; } = "";
        public string DIRECC { get; set; } = "";
        public string LOCALIDAD { get; set; } = "";
        public string TELEFONO { get; set; } = "";
        public string EMAIL { get; set; } = "";
        public string DES_PROV { get; set; } = "";
        public int CG_POST { get; set; } = 0;
    }
}
