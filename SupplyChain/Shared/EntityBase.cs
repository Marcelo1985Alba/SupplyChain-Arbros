using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

public class EntityBase<TId>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TId Id { get; set; }
}