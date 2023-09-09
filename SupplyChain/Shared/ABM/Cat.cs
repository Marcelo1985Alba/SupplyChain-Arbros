using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain;

[Table("Cat")]
public class Cat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CG_ORDEN { get; set; } = 0;

    public string DES_ORDEN { get; set; } = "";
}