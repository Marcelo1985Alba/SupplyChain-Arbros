using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain
{
    public class ClienteExterno : EntityBase
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Codigo")]
        public int CG_CLI { get; set; } = 0;
        [Display(Name = "Descripcion")]
        public string DESCRIPCION { get; set; } = "";
        public string CUIT { get; set; } = "";
        public string PROVINCIA { get; set; } = "";
        public string PAIS { get; set; } = "";
        public string RUBRO { get; set; } = "";
        public string MERCADO { get; set; } = "";
    }
}
