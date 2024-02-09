using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.ABM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class ProcedimientosRepository : Repository<Procedimiento, string>
    {
        public ProcedimientosRepository(AppDbContext appDb) : base(appDb)
        {

        }
    }
}

