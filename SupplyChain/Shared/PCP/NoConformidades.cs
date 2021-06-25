using SupplyChain.Shared.HelpersAtributo;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain
{
    [Table("NoConfor")]
    public class NoConformidades
    {
        [Key]
		public int Cg_NoConf { get; set; } = 0;
		//[Range(minimum: 1, maximum: 999999)]
		public int Cg_TipoNc { get; set; } = 0;
		public int Orden { get; set; } = 0;
		public string Observaciones { get; set; } = "";
		public DateTime? Fe_Ocurrencia { get; set; }
		public DateTime? Fe_Aprobacion { get; set; }
		public bool Aprob { get; set; } = false;
		public decimal Cg_Cli { get; set; } = 0;
		public string Cg_Prod { get; set; } = "";
		public int Cg_Orden { get; set; } = 0;
		public string Lote { get; set; } = "";
		public string Serie { get; set; } = "";
		public string Despacho { get; set; } = "";
		public decimal Cg_Ordf { get; set; } = 0;
		public decimal Pedido { get; set; } = 0;
		public int Cg_Cia { get; set; } = 0;
		public string Usuario { get; set; } = "";
		public int CG_PROVE { get; set; } = 0;
		public int OCOMPRA { get; set; } = 0;
		public decimal CANT { get; set; } = 0;
		public bool NOCONF { get; set; } = false;
		public DateTime? FE_EMIT { get; set; }
		public DateTime? FE_PREV { get; set; }
		public DateTime? FE_SOLUC { get; set; }
		public string DES_CLI { get; set; } = "";
		public string DES_PROVE { get; set; } = "";
		public string Comentarios { get; set; } = "";
		public DateTime? fe_implemen { get; set; }
		public DateTime? fe_cierre { get; set; }

		//public virtual string Des_TipoNc { get; set; } = "";
		//public virtual string Origen { get; set; } = "";

	}
}

