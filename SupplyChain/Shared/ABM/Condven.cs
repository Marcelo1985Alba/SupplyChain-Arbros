using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain;

[Table("Condven")]
public class Condven
{
    [Key] public string CG_CONDV { get; set; } = "";

    public int CG_PROVE { get; set; } = 0;
    public int DPD { get; set; } = 0;
    public int ORDEN { get; set; } = 0;
    public int CG_CLI { get; set; } = 0;
}