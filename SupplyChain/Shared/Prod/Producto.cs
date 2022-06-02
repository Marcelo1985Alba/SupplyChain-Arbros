using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.Models
{
    [Table("Prod")]
    public class Producto :  EntityBase<string>
    {
        [Key, Column("CG_PROD")]
        [ColumnaGridViewAtributo(Name = "Código producto")]
        public new string Id { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Nombre producto")]
        public string DES_PROD { get; set; }
        [ColumnaGridViewAtributo(Name = "Tipo insumo"), Range(1,13, ErrorMessage = "El tipo de insumo es requerido")]
        public int CG_ORDEN { get; set; }
        [ColumnaGridViewAtributo(Name = "Tipo producto"), Required(ErrorMessage = "El tipo es requerido")]
        public string TIPO { get; set; }
        [ColumnaGridViewAtributo(Name = "Unidad stock"), Required(ErrorMessage = "La unidad es requerida")]
        public string UNID { get; set; }
        [ColumnaGridViewAtributo(Name = "Factor de conversión")]
        public decimal? CG_DENSEG { get; set; }
        [ColumnaGridViewAtributo(Name = "Unidad comercial")]
        public string UNIDSEG { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Peso")]
        public decimal PESO { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Unidad peso")]
        public string UNIDPESO { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Especificaciones")]
        public string ESPECIF { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Norma")]
        public string NORMA { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Exige despacho")]
        public bool EXIGEDESPACHO { get; set; } = false;
        [ColumnaGridViewAtributo(Name = "Exige lote")]
        public bool EXIGELOTE { get; set; } = false;
        [ColumnaGridViewAtributo(Name = "Exige serie")]
        public bool EXIGESERIE { get; set; } = false;
        [ColumnaGridViewAtributo(Name = "Stock mínimo")]
        public bool EXIGEOA { get; set; } = false;
        [ColumnaGridViewAtributo(Name = "Orden de armado")]
        public decimal? STOCKMIN { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Lote óptimo compra")]
        public decimal? LOPTIMO { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Area fabricación")]
        public int? CG_AREA { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Línea fabricación"), Required(ErrorMessage = "El tipo es requerido")]
        public int? CG_LINEA { get; set; }
        [ColumnaGridViewAtributo(Name = "Tiempo fabricación")]
        public decimal? TIEMPOFAB { get; set; } = 15;
        [ColumnaGridViewAtributo(Name = "Costo fabricación")]
        public decimal? COSTO { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Costo fabricación terceros")]
        public decimal? COSTOTER { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Moneda costo")]
        public string MONEDA { get; set; } = "Dolares";
        [ColumnaGridViewAtributo(Name = "Celda fabricación"), Required(ErrorMessage = "La celda es requerida")]
        public string CG_CELDA { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Tipo área fabricación")]
        public int? CG_TIPOAREA { get; set; } = 0;
        public int? CG_CLAS { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "CAMPOCOM1")]
        public string CAMPOCOM1 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM2")]
        public string CAMPOCOM2 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM3")]
        public string CAMPOCOM3 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM4")]
        public string CAMPOCOM4 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM5")]
        public string CAMPOCOM5 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM6")]
        public string CAMPOCOM6 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM7")]
        public string CAMPOCOM7 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM8")]
        public string CAMPOCOM8 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM9")]
        public string CAMPOCOM9 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM10")]
        public string CAMPOCOM10 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM11")]
        public string CAMPOCOM11 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "CAMPOCOM12")]
        public string CAMPOCOM12 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Fecha ultima compra")]
        public DateTime? FE_UC { get; set; }
    }
}