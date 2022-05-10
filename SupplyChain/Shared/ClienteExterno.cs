using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain
{
    public class ClienteExterno : EntityBase<string>
    {
        [Display(Name = "Codigo")]
        public string CG_CLI { get; set; } = "";
        [Display(Name = "Descripcion")]
        public string DESCRIPCION { get; set; } = "";
        [Display(Name = "Cuit")]
        public string CUIT { get; set; } = "";
        public string PROVINCIA { get; set; } = "";
        public string PAIS { get; set; } = "";
        public string RUBRO { get; set; } = "";
        public string MERCADO { get; set; } = "";
    }
}
