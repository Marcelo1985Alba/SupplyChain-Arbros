using System.ComponentModel.DataAnnotations;

namespace SupplyChain;

public class ModeloOrdenFabricacionSE
{
    [Key] public int REGISTRO { get; set; }

    public string CG_ART { get; set; }
    public string DES_ART { get; set; }
    public string DESPACHO { get; set; }
    public decimal STOCK { get; set; }
    public string LOTE { get; set; }
    public int VALE { get; set; }
    public string UBICACION { get; set; }
    public int CG_LINEA { get; set; }
}