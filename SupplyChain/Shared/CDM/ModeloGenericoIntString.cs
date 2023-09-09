using System.ComponentModel.DataAnnotations;

namespace SupplyChain;

public class ModeloGenericoIntString
{
    [Key] public int ID { get; set; }

    public string TEXTO { get; set; }
}