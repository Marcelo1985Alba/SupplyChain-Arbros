using System.ComponentModel.DataAnnotations;

namespace SupplyChain;

public class ModeloGenericoStringString
{
    [Key] public string ID { get; set; }

    public string TEXTO { get; set; }
}