using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

[Table("Unidades")]
public class Unidades : EntityBase<string>
{
    [Key] [Column("UNID")] public new string Id { get; set; } = "";

    public string DES_UNID { get; set; } = "";
    public string TIPOUNID { get; set; } = "";
    public decimal CG_DENBASICA { get; set; } = 0;
    public decimal CODIGO { get; set; } = 0;

    [NotMapped] public bool GUARDADO { get; set; }

    [NotMapped] public bool ESNUEVO { get; set; }
}