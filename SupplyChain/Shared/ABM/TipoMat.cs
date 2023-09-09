using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain;

[Table("TipoMat")]
public class TipoMat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string TIPO { get; set; } = "";
}