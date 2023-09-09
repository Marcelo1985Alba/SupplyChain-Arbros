using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

public class vEstadPedidosAlta
{
    public int PEDIDO { get; set; }
    public DateTime FECHA { get; set; }
    public DateTime FECHA_DATE { get; set; }
    public DateTime? FECHA_PREVISTA { get; set; }
    public string CG_ART { get; set; }
    public string DES_ART { get; set; }
    public decimal STOCK { get; set; }
    public decimal UNIDEQUI { get; set; }
    public int MES { get; set; }
    public int ANIO { get; set; }

    [NotMapped] public int DiasAtraso { get; set; } = 0;
}