using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.Models
{
    [Table("Prove")]
    public class Proveedor : EntityBase
    {
        [Key]
        [Display(Name = "Codigo")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CG_PROVE { get; set; }
        [Display(Name = "Nombre del proveedor")]
        public string DES_PROVE { get; set; }
        //[Display(Name = "Observaciones")]
        //public string OBSERVACIONES { get; set; }
        [Display(Name = "Dirección")]
        public string DIRECC { get; set; }
        [Display(Name = "Localidad")]
        public string LOCALIDAD { get; set; }
        [Display(Name = "Codigo postal")]
        public int? CG_POST { get; set; }
        [Display(Name = "Provincia")]
        public int? CG_PROV { get; set; }
        //[Display(Name = "Nombre provincia")]
        //public string DES_PROV { get; set; }
        [Display(Name = "Pais")]
        public int? CG_PAIS { get; set; }
        [Display(Name = "Condición fiscal")]
        public string CG_COND { get; set; }
        [Display(Name = "Fecha alta")]
        public DateTime? FE_ALTA { get; set; }
        [Display(Name = "Nombre del contacto")]
        public string CONTACTO { get; set; }
        [Display(Name = "Teléfono")]
        public string TELEFONO { get; set; }
        [Display(Name = "FAX")]
        public string FAX { get; set; }
        [Display(Name = "Email")]
        public string EMAIL { get; set; }
        //[Display(Name = "Saldo")]
        //public decimal? SALDO { get; set; }
        //[Display(Name = "Crédito")]
        //public decimal? CREDITO { get; set; }
        [Display(Name = "Activo")]
        public string ACTIVO { get; set; } = "S";
        [Display(Name = "CUIT")]
        public string CUIT { get; set; }
        //[Display(Name = "IIBB")]
        //public string BRUTOS { get; set; }
        //[Display(Name = "Convenio")]
        //public int CM { get; set; }
        [Display(Name = "Bonificación")]
        public decimal? BONIFIC { get; set; }
        [Display(Name = "Moneda")]
        public string MONEDA { get; set; }
        [Display(Name = "Dirección de entrega")]
        public byte ENTREGA { get; set; }
        [Display(Name = "Código postal alfanumerico")]
        public string CG_POSTA { get; set; }
        //[Display(Name = "Porc.IVA especial")]
        //public decimal? PORCIVAESP { get; set; }
        //[Display(Name = "LEY19640")]
        //public int? LEY19640 { get; set; }
        [Display(Name = "Compañía")]
        public int? CG_CIA { get; set; }

        //public ICollection<Pedidos> Stocks { get; set; }

        //public virtual ICollection<Compra> Compras { get; set; }
    }
}
