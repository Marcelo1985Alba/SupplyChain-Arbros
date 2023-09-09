using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

public class vIngenieriaProductosFormulas
{
    public string CG_PROD { get; set; }
    public string DES_PROD { get; set; }
    public bool TIENE_FORM { get; set; }
    public bool FORM_ACTIVA { get; set; }

    [NotMapped] public decimal? COSTO { get; set; }
}