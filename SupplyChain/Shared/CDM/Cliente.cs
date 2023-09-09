using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

[Table("Cliente")]
public class Cliente : EntityBase<int>
{
    [Key]
    [Display(Name = "Codigo")]
    [Column("CG_CLI")]
    public int Id { get; set; } = 0;

    [Display(Name = "Dewcripcion")] public string DES_CLI { get; set; } = "";

    public string CUIT { get; set; } = "";
    public string DIRECC { get; set; } = "";
    public string LOCALIDAD { get; set; } = "";
    public string TELEFONO { get; set; } = "";
    public string EMAIL { get; set; } = "";
    public string DES_PROV { get; set; } = "";
    public int CG_POST { get; set; } = 0;
}