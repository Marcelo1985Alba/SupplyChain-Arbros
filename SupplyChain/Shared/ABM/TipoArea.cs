using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

[Table("TipoArea")]
public class TipoArea : EntityBase<int>
{
    [Key] [Column("CG_TIPOAREA")] public new int Id { get; set; } = 0;

    public string DES_TIPOAREA { get; set; } = "";

    [NotMapped] public bool GUARDADO { get; set; }

    [NotMapped] public bool ESNUEVO { get; set; }
}