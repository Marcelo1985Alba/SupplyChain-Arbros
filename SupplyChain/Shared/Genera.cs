using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
	public class Genera
	{
		public byte CG_CIA { get; set; }
		public string CAMP3 { get; set; }
		public decimal VALOR1 { get; set; }
		public decimal VALOR2 { get; set; }
		public string PUNTO_VENTA { get; set; }
		public decimal MAXVALOR { get; set; }
		public decimal ULTIMOCAMBIO { get; set; }
	}
}
