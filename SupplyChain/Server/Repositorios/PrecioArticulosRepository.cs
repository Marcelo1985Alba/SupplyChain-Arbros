using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PrecioArticulosRepository : Repository<PrecioArticulo, string>
    {
        public PrecioArticulosRepository(AppDbContext appDbContext) : base(appDbContext)
    {

    }
}
}
