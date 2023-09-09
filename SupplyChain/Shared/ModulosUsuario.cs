#nullable enable
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

[Table("Modulo_Usuario")]
public class ModulosUsuario : EntityBase<int>
{
    public int ModuloId { get; set; }
    public string UserId { get; set; }
}