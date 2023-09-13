using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class vDireccionesEntregaRepository : Repository<vDireccionesEntrega, int>
    {
        public vDireccionesEntregaRepository( AppDbContext appDbContext) :base( appDbContext)
        {

        }
    }
}
