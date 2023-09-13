using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class vTipoCambioRepository : Repository<vTipoCambio, int>
    {
        public vTipoCambioRepository( AppDbContext appDbContext) :base( appDbContext)
        {

        }
    }
}
