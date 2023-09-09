using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

[Table("Areas")]
public class Areas : EntityBase<int>
{
    [Key] [Column("CG_AREA")] public new int Id { get; set; } = 0;

    public string DES_AREA { get; set; } = "";
    public string CONTROLES { get; set; } = "";
    public int CG_TIPOAREA { get; set; } = 0;
    public int CG_PROVE { get; set; } = 0;
    public int CG_CIA { get; set; } = 0;

    [NotMapped] public bool GUARDADO { get; set; }

    [NotMapped] public bool ESNUEVO { get; set; }
}