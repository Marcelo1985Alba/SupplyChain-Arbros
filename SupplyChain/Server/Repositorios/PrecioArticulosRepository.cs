using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PrecioArticulosRepository : Repository<PreciosArticulos, string>
    {
        public PrecioArticulosRepository(AppDbContext appDbContext) : base(appDbContext)
    {

    }
}
}
