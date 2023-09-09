using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

public class Solution : EntityBase<string>
{
    [Key] [Column("Registro")] public new int Id { get; set; }

    public string CAMPO { get; set; }
    public string VALORC { get; set; }
    public string DESCRIP { get; set; }
}