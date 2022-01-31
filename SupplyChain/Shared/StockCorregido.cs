using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared
{
	[Table("DW_StockCorregido")]
    public class StockCorregido : EntityBase
    {
		[Key]
		public string CG_PROD { get; set; }
		public string DES_PROD { get; set; }
		public decimal CG_FORM { get; set; }
		public decimal CG_AREA { get; set; }
		public decimal CG_ORDEN { get; set; }
		public decimal STOCK { get; set; }
		public decimal COMP_DE_ENTRADA { get; set; } = 0;
		public decimal COMP_DE_SALIDA { get; set; }
		public decimal LOTE_OPTIMO { get; set; }
		public decimal STOCK_MINIMO { get; set; }
		public decimal LOTE_OP { get; set; }
		public decimal STOCK_MIN { get; set; }
		public decimal EN_PROCESO { get; set; }
	}
}
