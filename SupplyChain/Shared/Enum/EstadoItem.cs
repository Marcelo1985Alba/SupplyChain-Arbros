using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.Enum
{
    /// <summary>
    /// determinar el estado de un item para saber que hara contra la base de datos
    /// </summary>
    public enum EstadoItem
    {
        Agregado = 1,
        Modificado = 2,
        Eliminado = 3
    }
}
