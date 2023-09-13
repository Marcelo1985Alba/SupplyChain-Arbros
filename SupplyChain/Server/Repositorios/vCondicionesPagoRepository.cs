using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class vCondicionesPagoRepository : Repository<vCondicionesPago, int>
    {
        public vCondicionesPagoRepository( AppDbContext appDbContext) :base( appDbContext)
        {

        }
    }
}
