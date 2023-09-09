using System.ComponentModel.DataAnnotations;

namespace SupplyChain;

public class ModeloPedidosDespacho
{
    [Key] public string DESPACHO { get; set; }
}

public class ModeloPedidosLote
{
    [Key] public string LOTE { get; set; }
}