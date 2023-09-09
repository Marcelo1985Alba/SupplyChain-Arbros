using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Shared.PCP;

public class vPendienteFabricar
{
    [Key] public decimal? REGISTRO { get; set; } = 0;

    public decimal? PEDIDO { get; set; } = 0;
    public string CG_ART { get; set; } = "";
    public string DES_ART { get; set; } = "";
    public decimal? PREVISION { get; set; } = 0;
    public decimal? CANTPED { get; set; } = 0;
    public decimal? CALCULADO { get; set; } = 0;
    public decimal? CANTEMITIR { get; set; } = 0;
    public decimal? LOPTIMO { get; set; } = 0;
    public decimal? STOCK { get; set; } = 0;
    public decimal? STOCKMIN { get; set; } = 0;
    public int? CG_FORM { get; set; } = 0;
    public decimal? STOCKENT { get; set; } = 0;
    public decimal? COMP_EMITIDAS { get; set; } = 0;
    public string EXIGEOA { get; set; } = "";
}