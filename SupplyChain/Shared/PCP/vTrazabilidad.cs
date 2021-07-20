using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupplyChain.Shared.HelpersAtributo;
using SupplyChain.Shared.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.PCP
{
    public class vTrazabilidad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ColumnaGridViewAtributo(Name = "Registro")]
        public int? REGISTRO { get; set; }
        [ColumnaGridViewAtributo(Name = "Pedido")]
        public int? PEDIDO { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Nombre cliente")]
        public string DES_CLI { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Código artículo")]
        public string CG_ART { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Nombre artículo")]
        public string DES_ART { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Despacho")]
        public string DESPACHO { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Lote")]
        public string LOTE { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Fecha Vale")]
        public DateTime FE_MOV { get; set; }
        [ColumnaGridViewAtributo(Name = "Norma")]
        public string NORMA { get; set; }
        [ColumnaGridViewAtributo(Name = "Tipo de operación")]
        public int? TIPOO { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Línea fabricación")]
        public int? CG_LINEA { get; set; }
        [ColumnaGridViewAtributo(Name = "Remito")]
        public string REMITO { get; set; } = "0000-00000000";
    }
}
