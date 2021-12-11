using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PedidosRepository : Repository<Pedidos, int>
    {
        public PedidosRepository(AppDbContext db) : base(db)
        {
        }


        public async Task<decimal> GetRecepSumByOcMp(int oCompra, string cg_mat)
        {
            return await DbSet.Where(p => p.TIPOO == 5 && p.OCOMPRA == oCompra && p.CG_ART == cg_mat)
                    .SumAsync(p => p.STOCK) ?? 0;
        }
    }
}
