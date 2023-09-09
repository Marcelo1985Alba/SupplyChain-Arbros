using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

[Table("PRESUPUESTO_MOTIVOS")]
public class MotivosPresupuesto : EntityBase<int>
{
    //public int Id { get; set; }
    public string Motivo { get; set; }
}