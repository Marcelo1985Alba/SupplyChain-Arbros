using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
	public class MovimientoStockSP
	{
		public DateTime Fecha { get; set; }
		public string Codigo { get; set; }
		public string Descripcion { get; set; }
		public string UnidMed { get; set; }
		public string UnidComer { get; set; }
		public string Despacho { get; set; }
		public string Lote { get; set; }
		public string Serie { get; set; }
		public int Cg_Orden { get; set; }
		public string Tipo_Insumo { get; set; }
		public int Codigo_Deposito { get; set; }
		public string Deposito { get; set; }
		public string Ubicacion { get; set; }
		public string Tipo_Deposito { get; set; }
		public string Tipo { get; set; }
		public decimal Entradas { get; set; }
		public decimal Salidas { get; set; }
		public decimal Saldo { get; set; }
		public string Concepto { get; set; }
		public decimal Vale { get; set; }
		public string Remito { get; set; }
		public string Comprob { get; set; }
		public string Factura { get; set; }
		public decimal Importe1 { get; set; }
		public int Tipoo { get; set; }
		public int Cg_Cli { get; set; }
		public int Cg_Prove { get; set; }
		public string Cliente { get; set; }
		public string Proveedor { get; set; }
		public string Observaciones { get; set; }
		public string Acts { get; set; }
		[Key]
		public decimal Registro { get; set; }
		public decimal Cg_Ordf { get; set; }
	}
}
