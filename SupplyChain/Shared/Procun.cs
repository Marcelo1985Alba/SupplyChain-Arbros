using SupplyChain.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared
{
	[Table("Procun")]
	public class Procun : EntityBase<decimal>
	{
		[Key, Column("REGISTRO"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public new decimal Id { get; set; } = 0;
		//public new decimal Id { get; set; } = 0;
        //[ColumnaGridViewAtributo(Name = "Orden proceso"),Range(1, 13, ErrorMessage = "El Orden requerido")]
        public int ORDEN { get; set; }
        [ColumnaGridViewAtributo(Name = "Código producto"), Required(ErrorMessage = "El producto es requerido")]
        public string CG_PROD { get; set; }
        //[ColumnaGridViewAtributo(Name = "Form"), Range(1, 13, ErrorMessage = "El Form es requerido")]
        public int CG_FORM { get; set; }
        [ColumnaGridViewAtributo(Name = "Área"), Range(1, 13, ErrorMessage = "El tipo de Área es requerido")]
        public int CG_AREA { get; set; }
        [ColumnaGridViewAtributo(Name = "Línea"), Range(1, 220, ErrorMessage = "El tipo de Línea es requerido")]
        public int CG_LINEA { get; set; }
		[ColumnaGridViewAtributo(Name	= "Celda"), Required(ErrorMessage = "El tipo de Celda es requerido")]
        public string CG_CELDA { get; set; }
        //public string DES_AREA { get; set; }
		[ColumnaGridViewAtributo(Name = "Proceso"), Required(ErrorMessage = "El proceso es requerido")]
        public string PROCESO { get; set; }
        public string? DESCRIP { get; set; }
		public string? OBSERV {		get; set; }
		public string? DESPROC { get; set; }
		[ColumnaGridViewAtributo(Name = "Tiempo"), Range(1,20000, ErrorMessage = "El tiempo es requerido")]
		public decimal TIEMPO1 { get; set; }
		[ColumnaGridViewAtributo(Name = "TS1"), Range(1,20000, ErrorMessage = "El ts1 es requerido")]
        public decimal TS1 { get; set; }
        public int FRECU { get; set; }
		public int CG_CALI1 { get; set; }
		[ColumnaGridViewAtributo(Name = "Proporc"), Required(ErrorMessage = "La proporción es requerido")]
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
		public DateTime? FECHA { get; set; }
		public decimal COSTO { get; set; }
		public decimal VALOR4 { get; set; }
		public decimal COSTOCOMB { get; set; }
		public decimal COSTOENERG { get; set; }
		public decimal PLANTEL { get; set; }
		public string? CG_CATEOP { get; set; }
		public decimal COSTAC { get; set; }
		public decimal OCUPACION { get; set; }
		public decimal COEFI { get; set; }
		public string? TAREAPROC { get; set; }
		public bool ESTANDAR { get; set; }
		public int RELEVAN { get; set; }
		public decimal REVISION { get; set; }
		public string USUARIO { get; set; }
		public string? AUTORIZA { get; set; }
		[NotMapped]
		public bool GUARDADO { get; set; }
		[NotMapped]
		public bool ESNUEVO { get; set; }
		[NotMapped]
		public string Des_Prod { get; set; }
		
	}
}