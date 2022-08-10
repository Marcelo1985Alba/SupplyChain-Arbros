using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class vTransportesRepository : Repository<vTransporte, int>
    {
        public vTransportesRepository( AppDbContext appDbContext) :base( appDbContext)
        {

        }
    }
}
