using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain;

[Table("NoConfor_Acciones")]
public class NoConformidadesAcciones
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Cg_NoConfAcc { get; set; } = 0;

    public int Cg_NoConf { get; set; } = 0;

//		[Required]
    [Range(1, 999999, ErrorMessage = "Debe Indicar el Tipo de Acción")]
    public int Orden { get; set; } = 0;

    public string DesOrden { get; set; } = "";

    [Required(ErrorMessage = "Debe Indicar una Observación")]
    [StringLength(600)]
    public string Observaciones { get; set; } = "";

    public DateTime? Fe_Ocurrencia { get; set; }
    public string Usuario { get; set; } = "";
}