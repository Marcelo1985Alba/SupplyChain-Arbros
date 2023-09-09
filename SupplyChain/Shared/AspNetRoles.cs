using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

[Table("aspnetroles")]
public class AspNetRoles : EntityBase<string>
{
    [Key] [Column("Id")] public new string Id { get; set; } = "";

    public string Name { get; set; } = "";
    public string NormalizedName { get; set; } = "";
    public string ConcurrencyStamp { get; set; } = "";
}