using System.ComponentModel.DataAnnotations;

namespace SupplyChain;

public class ModeloOrdenFabricacionHojaRuta
{
    [Key] public int ORDEN { get; set; }

    public string PROCESO { get; set; }
    public string DESCRIP { get; set; }
    public string CG_CELDA { get; set; }
    public string DES_CELDA { get; set; }
    public decimal TIEMPO_TOTAL { get; set; }
    public string PROPORC { get; set; }
    public decimal TIEMPO1 { get; set; }
}