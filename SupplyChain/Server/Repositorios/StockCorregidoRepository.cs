using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class StockCorregidoRepository : Repository<StockCorregido, string>
    {
        public StockCorregidoRepository(AppDbContext db) : base(db)
        {

        }

    }
}
