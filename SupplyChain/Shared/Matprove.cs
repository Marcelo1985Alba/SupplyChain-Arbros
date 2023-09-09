using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.Models;

[Table("Matprove")]
public class Matprove : EntityBase<int>
{
    [Key] [Column("REGISTRO")] public new int Id { get; set; }

    public int NROCLTE { get; set; }
    public string CG_MAT { get; set; }

    [Display(Name = "Insumo")] public string CG_MAT1 { get; set; }

    public string DES_MAT1 { get; set; }
    public decimal CANT { get; set; }
    public string UNID { get; set; }
    public decimal CG_DEN { get; set; }
    public decimal CANTAUTOR { get; set; }
    public string UNID1 { get; set; }
    public int ENTREGA { get; set; }
    public decimal PRECIO { get; set; }
    public decimal PRECIO2 { get; set; }
    public DateTime? FE_PREC { get; set; }

    [Display(Name = "Fecha Precio")] public string MONEDA { get; set; }

    public int DIASVIGE { get; set; }
    public string CONDPREC { get; set; }
    public string CONDVEN { get; set; }
    public decimal BON11 { get; set; }
    public decimal CANTMIN { get; set; }
    public decimal CANTLOTE { get; set; }
    public string USUARIO { get; set; }
    public int ACTIVO { get; set; }
    public DateTime? FE_REG { get; set; }
}