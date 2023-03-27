using SupplyChain.Shared;
using SupplyChain.Shared.HelpersAtributo;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain.Shared.Models
{
	public class Matprove_busquedaprove
	{
        [Key]
        public int? REGISTRO { get; set; }
        public int NROCLTE { get; set; }
		public int CG_PROVE { get; set; }
		public string? DES_PROVE { get; set; }
		public string CG_MAT { get; set; }
		public string UNID { get; set; }
		public decimal CG_DEN { get; set; }
		public decimal CANTAUTOR { get; set; }
		public string UNID1 { get; set; }
		public int ENTREGA { get; set; }
		public decimal PRECIO { get; set; }
		public DateTime? FE_PREC { get; set; }
		public string MONEDA { get; set; }
		public int DIASVIGE { get; set; }
		public string CONDPREC { get; set; }
		public string CONDVEN { get; set; }
		
	}
}
