using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vPedidoAlta
    {
        public DateTime FECHA_PEDIDO { get; set; }
        public DateTime FECHA_ALTA { get; set; }
        public int PEDIDO { get; set; }
        public int CG_ORDF { get; set; }
        public int CG_CLI { get; set; }
        public int CG_COND_ENTREGA { get; set; }
        public int CG_TRANS { get; set; }
        public string DIRENT { get; set; }
        public int DPP { get; set; }
        public string DES_CLI { get; set; }
        public string CG_ART { get; set; }
        public string DES_ART { get; set; }
        public decimal STOCK { get; set; }
        public string UNID { get; set; }
        public decimal UNIDEQUI_UNIT { get; set; }
        public string MONEDA { get; set; } = "PESOS";
        public decimal? IMPORTE1 { get; set; } = 0;
        public decimal? IMPORTE2 { get; set; }
        public decimal? IMPORTE3 { get; set; }
        public decimal? IMPORTE4 { get; set; }
        public decimal? IMPORTE6 { get; set; } = 0;
        public decimal? DESCUENTO { get; set; } = 0;
        public decimal? BONIFIC { get; set; } = 0;
        public decimal? VA_INDIC { get; set; } = 1;
        public string LOTE { get; set; } = "";
        public string SERIE { get; set; } = "";
        public string DESPACHO { get; set; } = "";
        public DateTime FECHA_PREVISTA { get; set; }
        public string CAMPOCOM1 { get; set; } = "";
        public string CAMPOCOM2 { get; set; } = "";
        public string CAMPOCOM3 { get; set; } = "";
        public string CAMPOCOM4 { get; set; } = "";
        public string CAMPOCOM5 { get; set; } = "";
        public string CAMPOCOM6 { get; set; } = "";
        public string CAMPOCOM7 { get; set; } = "";
        public string CAMPOCOM8 { get; set; } = "";
    }
}
