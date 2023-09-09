using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

[Table("Semaforo")]
public class Semaforo : EntityBase<int>
{
    //public int ID { get; set; }
    public string COLOR { get; set; }
    public int? ASIGNA { get; set; } = 0;
}