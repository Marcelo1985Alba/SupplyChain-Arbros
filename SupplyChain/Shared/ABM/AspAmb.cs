using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

[Table("AspectosAmbientales")]
public class AspAmb : EntityBase<int>
{
    [Key] [Column("Id")] public new int Id { get; set; } = 0;

    public string descripcion { get; set; } = "";

    [NotMapped] public bool GUARDADO { get; set; }

    [NotMapped] public bool ESNUEVO { get; set; }
}