using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.Enum
{
    public enum EstadoPedido
    {
        AConfirmar = 1,
        Confirmado = 2,
        EnProduccion = 3,
        TotalidadComponentes = 4,
        Armado = 5,
        PendienteRemitir = 6,
        AEntregar = 7,
        Entregado = 8,
        Facturado = 9,
        Anulado = 10,
        TodosPendientes= 11,
        SinCargo=12,
        Todos = 100,

    }
}
