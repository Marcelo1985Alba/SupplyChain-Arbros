using SupplyChain.Shared.PCP;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.Models
{

	public class ItemAbastecimiento
	{
		public string CG_ART { get; set; }
		public string LOTE {get;set;}
		public string SERIE {get;set;}
		public string DESPACHO {get;set;}
		public string UBICACION {get;set;}
		public string DES_ART {get;set;}
		public decimal CANTPED {get;set;}
		public decimal CANTENT {get;set;}
        public decimal CANTEMP { get; set; }
        public decimal STOCK {get;set;}
		public string UNID {get;set;}
		public int CG_DEP {get;set;}
		public int CG_ORDEN {get;set;}
		//public int CG_ORDF {get;set;}
		//public int PEDIDO {get;set;}
		public string OBSERITEM {get;set;}
        public string AVISO { get; set; }

		[NotMapped] public vResumenStock ResumenStock { get; set; } = new vResumenStock();
        /// <summary>
        /// Stock del deposito de insumos y reserva de todos los despachos
        /// </summary>
        [NotMapped] public decimal StockReal { get; set; } = 0;

        /// <summary>
        /// Stock para una determinada orden orden del insumo 
        /// </summary>
        [NotMapped] public decimal Reserva { get; set; } = 0;
    }
}
