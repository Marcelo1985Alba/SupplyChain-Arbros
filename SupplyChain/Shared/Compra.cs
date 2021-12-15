using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.Models
{
    [Table("Compras")]
    public class Compra : EntityBase
    {
        [Display(Name = "Número")]
        public int NUMERO { get; set; }
        [Display(Name = "Fecha emisión")]
        public DateTime FE_EMIT { get; set; }
        [Display(Name = "Código")]
        public string CG_MAT { get; set; }
        [Display(Name = "Nombre producto")]
        public string DES_MAT { get; set; }
        [Display(Name = "Necesario")]
        public decimal? NECESARIO { get; set; }
        [Display(Name = "Solicitado")]
        public decimal? SOLICITADO { get; set; }
        [Display(Name = "Autorizado")]
        public decimal? AUTORIZADO { get; set; }
        [Display(Name = "Unidad stock")]
        public string UNID { get; set; }
        [Display(Name = "Unidad compra")]
        public string UNID1 { get; set; }
        [Display(Name = "Factor conversión")]
        public decimal? CG_DEN { get; set; }
        [Display(Name = "Precio unidad compra")]
        public decimal? PRECIO { get; set; }
        [Display(Name = "Descuento")]
        public decimal? BON { get; set; }
        [Display(Name = "Precio con descuento")]
        public decimal? PRECIONETO { get; set; }
        [Display(Name = "Precio total")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? PRECIOTOT { get; set; }
        [Display(Name = "Moneda")]
        public string MONEDA { get; set; }
        //[ColumnaGridViewAtributo(Name = "Proveedor")]
        //public int NROCLTE { get; set; }
        [Display(Name = "Nombre proveedor")]
        public string DES_PROVE { get; set; }
        [Display(Name = "Fecha prevista entrega")]
        public DateTime? FE_PREV { get; set; }
        [Display(Name = "Fecha real entrega")]
        public DateTime? FE_REAL { get; set; }
        [Display(Name = "Fecha vencimiento")]
        public DateTime? FE_VENC { get; set; }
        [Display(Name = "Fecha cierre")]
        public DateTime? FE_CIERRE { get; set; }
        //[ColumnaGridViewAtributo(Name = "Fecha precio")]
        //public DateTime FE_PREC { get; set; }
        [Display(Name = "Condiciones compra")]
        public string CONDVEN { get; set; }
        //[ColumnaGridViewAtributo(Name = "Condiciones precio")]
        //public string CONDPREC { get; set; }
        //[ColumnaGridViewAtributo(Name = "Condiciones importación")]
        //public string CONDVENEX { get; set; }
        [Display(Name = "Especific.particulares")]
        public string ESPECIFICA { get; set; }
        //[ColumnaGridViewAtributo(Name = "Especific.grales")]
        //public string ESTEGEN { get; set; }
        [Display(Name = "Preparación")]
        public bool ABIERTOPREPARACION { get; set; }
        [Display(Name = "Fecha requisición")]
        public DateTime? FE_REQ { get; set; }
        [Display(Name = "Fecha autorización")]
        public DateTime? FE_AUTREQ { get; set; }
        [Display(Name = "Proveedor")]
        public int NROCLTE { get; set; }
        [Display(Name = "Proveedor")]
        public int? CG_PROVEREQ { get; set; }
        [Display(Name = "Observaciones")]
        public string OBSEREQ { get; set; }
        [Display(Name = "Tilde")]
        public string MARCAREQ { get; set; }
        [Display(Name = "Avance")]
        public decimal? AVANCE { get; set; }
        [Display(Name = "Texto observado")]
        public string TXTOBSERVADO { get; set; }
        [Display(Name = "Texto corregido")]
        public string TXTCORREGIDO { get; set; }
        [Display(Name = "Autorizador")]
        public string USUARIO_AUT { get; set; }
        [Display(Name = "Fecha autorización")]
        public DateTime? FE_AUT { get; set; }
        //[ColumnaGridViewAtributo(Name = "Fecha cierre")]
        //public DateTime FE_CIERREQ { get; set; }
        [Display(Name = "Usuario requisidor")]
        public string USUREQ { get; set; }
        [Display(Name = "Observaciones")]
        public string OBSERVACIONES { get; set; }
        [Display(Name = "Usuario")]
        public string USUARIO { get; set; }
        [Display(Name = "Fecha registro")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime FE_REG { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Registro")]
        public int REGISTRO { get; set; }
        [Display(Name = "Compañía")]
        public int? CG_CIA { get; set; }

        [NotMapped]
        public decimal? PENDIENTE { get; set; } = 0;
        [NotMapped]
        public virtual Proveedor ProveedorNavigation { get; set; }
    }
}
