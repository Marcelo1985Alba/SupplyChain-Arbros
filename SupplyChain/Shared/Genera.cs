using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

public class Genera : EntityBase<string>
{
    [Key] [Column("CAMP3")] public new string Id { get; set; }

    [Key] public byte CG_CIA { get; set; }

    public decimal VALOR1 { get; set; }
    public decimal VALOR2 { get; set; }

    [Key] public string PUNTO_VENTA { get; set; }

    public decimal MAXVALOR { get; set; }
    public decimal ULTIMOCAMBIO { get; set; }
}