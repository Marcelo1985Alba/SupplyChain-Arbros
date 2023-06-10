using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared
{
    [Table("Form2")]
    public class Formula : EntityBase<int>
    {
        [Key, Column("Registro")]
        new public int Id { get; set; }
        public string Cg_Prod { get; set; }
        
        public int CG_FORM { get; set; }
        public string Cg_Mat { get; set; }
        public string Cg_Se { get; set; }
        public int CG_FORM_SE { get; set; }
        public decimal CANT_MAT { get; set; }
        public int CG_CLAS { get; set; }
        public string ACTIVO { get; set; }
        public int CG_DEC { get; set; }
        public int CG_DEC_OF { get; set; }
        public DateTime FE_FORM { get; set; }
        public decimal MERMA { get; set; }
        public decimal CANTIDAD { get; set; }
        public int REVISION { get; set; }
        public DateTime FE_VIGE { get; set; }
        public string CG_GRUPOMP { get; set; }
        public string OBSERV { get; set; }
        public int TIPO { get; set; }
        public bool Diferencial { get; set; }
        public int CATEGORIA { get; set; }
        public string USUARIO { get; set; }
        public string DES_FORM { get; set; }
        public string DES_REVISION { get; set; }
        public string UNIPROD { get; set; }
        public string UNIMAT { get; set; }
        public decimal DOSIS { get; set; }
        public decimal REVISION_SE { get; set; }
        public decimal CANT_BASE { get; set; }
        public decimal CANT_MAT_BASE { get; set; }
        public decimal CANTIDAD_BASE { get; set; }
        public string TIPOFORM { get; set; }
        public int CG_ORDEN { get; set; }
        public string NOMBREFOTO { get; set; }
        public string FOTO { get; set; }
        public string AUTORIZA { get; set; }
    }
}
