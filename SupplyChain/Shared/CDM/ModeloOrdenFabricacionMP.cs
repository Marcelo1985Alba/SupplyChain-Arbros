using System.ComponentModel.DataAnnotations;

namespace SupplyChain;

public class ModeloOrdenFabricacionMP
{
    [Key] public int REGISTRO { get; set; }

    public string CG_ART { get; set; }
    public string DES_ART { get; set; }
    public decimal STOCK { get; set; }
    public string LOTE { get; set; }
    public string DESPACHO { get; set; }
    public string SERIE { get; set; }
}