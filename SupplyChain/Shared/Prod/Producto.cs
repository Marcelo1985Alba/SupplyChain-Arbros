using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.Models
{
    [Table("Prod")]
    public class Producto :  EntityBase
    {
        [Key]
        [ColumnaGridViewAtributo(Name = "Código producto"), Required(ErrorMessage = "El codigo es requerida")]
        public string CG_PROD { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Nombre producto"), Required(ErrorMessage = "La descripcion es requerida")]
        public string DES_PROD { get; set; }
        [ColumnaGridViewAtributo(Name = "Tipo insumo"), Range(1,15, ErrorMessage = "El tipo de insumo es requerido")]
        public int CG_ORDEN { get; set; }
        [ColumnaGridViewAtributo(Name = "Tipo producto")]
        public string TIPO { get; set; }
        [ColumnaGridViewAtributo(Name = "Unidad stock")]
        public string UNID { get; set; }
        [ColumnaGridViewAtributo(Name = "Factor de conversión")]
        public decimal? CG_DENSEG { get; set; }
        [ColumnaGridViewAtributo(Name = "Unidad comercial")]
        public string UNIDSEG { get; set; }
        [ColumnaGridViewAtributo(Name = "Peso")]
        public decimal PESO { get; set; }
        [ColumnaGridViewAtributo(Name = "Unidad peso")]
        public string UNIDPESO { get; set; }
        [ColumnaGridViewAtributo(Name = "Especificaciones")]
        public string ESPECIF { get; set; }
        [ColumnaGridViewAtributo(Name = "Norma")]
        public string NORMA { get; set; }
        [ColumnaGridViewAtributo(Name = "Exige despacho")]
        public bool EXIGEDESPACHO { get; set; }
        [ColumnaGridViewAtributo(Name = "Exige lote")]
        public bool EXIGELOTE { get; set; }
        [ColumnaGridViewAtributo(Name = "Exige serie")]
        public bool EXIGESERIE { get; set; }
        [ColumnaGridViewAtributo(Name = "Stock mínimo")]
        //public int? EXIGEOA { get; set; }
        public bool EXIGEOA { get; set; }
        [ColumnaGridViewAtributo(Name = "Orden de armado")]
        public decimal? STOCKMIN { get; set; }
        [ColumnaGridViewAtributo(Name = "Lote óptimo compra")]
        public decimal? LOPTIMO { get; set; }
        [ColumnaGridViewAtributo(Name = "Area fabricación")]
        public int? CG_AREA { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Línea fabricación")]
        public int? CG_LINEA { get; set; }
        //[ColumnaGridViewAtributo(Name = "Activo")]
        //public string ACTIVO { get; set; }
        [ColumnaGridViewAtributo(Name = "Tiempo fabricación")]
        public decimal? TIEMPOFAB { get; set; }
        //public decimal IMPA1 { get; set; } = 0;
        //public decimal IMPA2 { get; set; } = 0;
        //public decimal IMPB1 { get; set; } = 0;
        //public decimal IMPB2 { get; set; } = 0;
        //public decimal IMPC1 { get; set; } = 0;
        //public decimal IMPC2 { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Costo fabricación")]
        public decimal? COSTO { get; set; }
        [ColumnaGridViewAtributo(Name = "Costo fabricación terceros")]
        public decimal? COSTOTER { get; set; }
        [ColumnaGridViewAtributo(Name = "Moneda costo")]
        public string MONEDA { get; set; }
        //public decimal COSTOUCLOCAL { get; set; } = 0;
        //public decimal COSTOUCDOL { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Celda fabricación")]
        public string CG_CELDA { get; set; }
        [ColumnaGridViewAtributo(Name = "Tipo área fabricación")]
        public int? CG_TIPOAREA { get; set; }
        public int? CG_CLAS { get; set; }

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
        //public decimal CG_CUENT { get; set; } = 0;
        //[ColumnaGridViewAtributo(Name = "Cuenta contable")]
        //public decimal? CG_CUENT1 { get; set; }
        //[ColumnaGridViewAtributo(Name = "Unidad equivalente costo")]
        //public decimal? UNIDEQUI { get; set; }
        //[ColumnaGridViewAtributo(Name = "Costo ultima compra")]
        //public decimal? COSTOUC { get; set; }
        //[ColumnaGridViewAtributo(Name = "Moneda ultima compra")]
        //public string MONEDAUC { get; set; }
        //[ColumnaGridViewAtributo(Name = "Costo ultima compra")]
        //public decimal? COSTOUC1 { get; set; }
        [ColumnaGridViewAtributo(Name = "Fecha ultima compra")]
        public DateTime? FE_UC { get; set; }
        //[ColumnaGridViewAtributo(Name = "Usuario")]
        //public string USUARIO { get; set; }
        //[ColumnaGridViewAtributo(Name = "Fecha registro")]
        //public DateTime? FE_REG { get; set; }
        //[ColumnaGridViewAtributo(Name = "Compañía")]
        //public int? CG_CIA { get; set; }
    }
}