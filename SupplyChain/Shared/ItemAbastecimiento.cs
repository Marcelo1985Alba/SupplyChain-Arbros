using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared.PCP;

namespace SupplyChain.Shared.Models;

public class ItemAbastecimiento
{
    public string CG_ART { get; set; }
    public string LOTE { get; set; }
    public string SERIE { get; set; }
    public string DESPACHO { get; set; }
    public string UBICACION { get; set; }
    public string DES_ART { get; set; }
    public decimal CANTPED { get; set; }
    public decimal CANTENT { get; set; }
    public decimal CANTEMP { get; set; }
    public decimal STOCK { get; set; }
    public string UNID { get; set; }
    public int CG_DEP { get; set; }

    public int CG_ORDEN { get; set; }

    //public int CG_ORDF {get;set;}
    //public int PEDIDO {get;set;}
    public string OBSERITEM { get; set; }
    public string AVISO { get; set; }

    [NotMapped] public vResumenStock ResumenStock { get; set; } = new();
}