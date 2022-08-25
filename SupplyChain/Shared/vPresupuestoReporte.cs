using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vPresupuestoReporte
	{
		public int PRESUPUESTO { get; set; }
		public DateTime FECHAS_PRESUP { get; set; }
		public decimal BONIFIC { get; set; }
		public int CG_CLI { get; set; }
		public string CLIENTE { get; set; }
		public string CALLE_CLIENTE { get; set; }
		public string CP_CLIENTE { get; set; }
		public string DIRENT { get; set; }
		public string CONDICION_PAGO { get; set; }
		public string CONDICION_ENTREGA { get; set; }
		public string PROVINCIA_CLIENTE { get; set; }
		public string TELEFONO_CLIENTE { get; set; }
		public string EMAIL_CLIENTE { get; set; }
		public string CG_ART { get; set; }
		public string ARTICULO { get; set; }
		public decimal CANTIDAD { get; set; }
		public decimal PORC_DESCUENTO { get; set; }
		public decimal PREC_UNIT { get; set; }
		public decimal TOTAL_ITEM { get; set; }
		public string MONEDA { get; set; }
		public decimal TOTAL_PRESUPUESTO { get; set; }
		public string Construccion { get; set; }
		public string Marca { get; set; }
	}
}
