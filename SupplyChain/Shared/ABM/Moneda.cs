using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain;

[Table("Monedas")]
public class Moneda
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string MONEDA { get; set; } = "";

    public string TIPO { get; set; } = "";
    public string SIMBOLO { get; set; } = "";
    public string CODIGO { get; set; } = "";
}