using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
	[Table("Procun")]
	public class Procun
	{
		public int ORDEN { get; set; }
		public string CG_PROD { get; set; }
		public int CG_FORM { get; set; }
		public int CG_AREA { get; set; }
		public int CG_LINEA { get; set; }
		public string CG_CELDA { get; set; }
		public string PROCESO { get; set; }
		public string DESCRIP { get; set; }
		public string OBSERV { get; set; }
		public string DESPROC { get; set; }
		public decimal TIEMPO1 { get; set; }
		public decimal TS1 { get; set; }
		public int FRECU { get; set; }
		public int CG_CALI1 { get; set; }
		public string PROPORC { get; set; }
		public decimal TOLE1 { get; set; }
		public int CG_CALI2 { get; set; }
		public decimal VALOR1 { get; set; }
		public decimal TOLE2 { get; set; }
		public decimal CG_CALI3 { get; set; }
		public decimal VALOR2 { get; set; }
		public decimal TOLE3 { get; set; }
		public int CG_CALI4 { get; set; }
		public decimal VALOR3 { get; set; }
		public decimal TOLE4 { get; set; }
		public int CG_CALI5 { get; set; }
		public int CG_CALI6 { get; set; }
		public int CG_CALI7 { get; set; }
		public int CG_OPER { get; set; }
		public DateTime FECHA { get; set; }
		public decimal COSTO { get; set; }
		public decimal VALOR4 { get; set; }
		public decimal COSTOCOMB { get; set; }
		public decimal COSTOENERG { get; set; }
		public decimal REGISTRO { get; set; }
		public decimal PLANTEL { get; set; }
		public string CG_CATEOP { get; set; }
		public decimal COSTAC { get; set; }
		public decimal OCUPACION { get; set; }
		public decimal COEFI { get; set; }
		public string TAREAPROC { get; set; }
		public bool ESTANDAR { get; set; }
		public int RELEVAN { get; set; }
		public decimal REVISION { get; set; }
		public string USUARIO { get; set; }
		public string AUTORIZA { get; set; }
	}
}