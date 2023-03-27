using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.Enum
{//  1.Solicitar cotizacion
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
       SolicitarCotizacion=1,
       PendienteGenerarCompra=2,
       AEsperaCotizacion=3,
       PendienteEntrega=4,
       Pagada=5,
       Vencida=6,
       Cerrada=7,
       
       Todos = 100,
    }
}
