using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
	public class StockSP
	{
		public string Codigo { get; set; }
		public string Descripcion { get; set; }
		public string UnidMed { get; set; }
		public string UnidComer { get; set; }
		public string Despacho { get; set; }
		public string Lote { get; set; }
		public string Serie { get; set; }
		public string Tipo_Insumo { get; set; }
		public string Deposito { get; set; }
		public string Ubicacion { get; set; }
		public int Cg_Clas { get; set; }
		public string Tipo_Deposito { get; set; }
		public string Tipo { get; set; }
		public decimal Stock_Fisico { get; set; }
		public decimal Stock_Fisico_anterior { get; set; }
		public decimal? Pendiente_Entrada { get; set; }
		public decimal? Pendiente_Salida { get; set; }
		public decimal? Stock_Seguridad { get; set; }
		public decimal? Stock_Corregido { get; set; }
		public decimal Pesos { get; set; }
		public decimal Dolares { get; set; }
		public int Codigo_Deposito { get; set; }
	}
}
