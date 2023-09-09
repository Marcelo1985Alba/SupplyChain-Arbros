#nullable enable
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

[Table("Modulos")]
public class Modulo : EntityBase<int>
{
    public string Descripcion { get; set; }
    public int? ParentId { get; set; }
    public bool TieneChild { get; set; }
    public string? Url { get; set; }
    public string? IconCss { get; set; }
}