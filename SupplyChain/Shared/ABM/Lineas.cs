using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

[Table("Lineas")]
public class Lineas : EntityBase<int>
{
    [Key] [Column("CG_LINEA")] public new int Id { get; set; } = 0;

    public string DES_LINEA { get; set; } = "";
    public decimal FACTOR { get; set; } = 0;
    public string RESP { get; set; } = "";

    [NotMapped] public bool GUARDADO { get; set; }

    [NotMapped] public bool ESNUEVO { get; set; }
}