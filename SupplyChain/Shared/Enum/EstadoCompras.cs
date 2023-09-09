namespace SupplyChain.Shared.Enum;

//  1.Solicitar cotizacion
//2.Pendiente de cotizacion
//3.Pendiente de autorizar compra
//4.Pendiente de entrega
//5.Vencida
//6.Cerrada
//7.Pendiente de Pago
//8.Pagada(tiene recibo
//9.Anulada
public enum EstadoCompras
{
    PendEmSolCot = 1,
    PendEmisionOC = 2,
    PendEntFecha = 3,
    PendEntVenc = 4,
    RecParcialPendPago = 5,
    RecTotalPendPago = 6,
    PagRecibida = 7,
    Cerrada = 8,
    TodosPendientes = 9,
    Todos = 100
}