using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain;

[Table("CateOperarios")]
public class CatOpe
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string CG_CATEOP { get; set; } = "";

    public string DES_CATEOP { get; set; } = "";
    public decimal VALOR_HORA { get; set; } = 0;
    public string MONEDA { get; set; } = "";

    //cg_cia
}