using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.Enum
{
    public enum TipoFiltro
    {
        Todos = 0,
        Pendientes = 1,
        NoPendientes = 2,
        PendEmSolCot=3,
        PendEmisionOC=4,
        PendEntFecha=5,
        PendEntVenc=6,
        RecParcialPendPago=7,
        RecTotalPendPago=8,
        PagRecibida=9,
        Cerrada=10
    }
}
