using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain;

[Table("NoConfor_ListaAcciones")]
public class NoConformidadesListaAcciones
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Tipoaccion { get; set; } = 0;

    [Required(ErrorMessage = "Debe Indicar una Descripción")]
    [StringLength(50)]
    public string Texto { get; set; } = "";
}