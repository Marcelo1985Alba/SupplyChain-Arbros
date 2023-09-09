using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.PCP;

[Table("EstadosCargaMaquinas")]
public class EstadosCargaMaquina
{
    [Key] public int CG_ESTADO { get; set; }

    public string ESTADO { get; set; }
}