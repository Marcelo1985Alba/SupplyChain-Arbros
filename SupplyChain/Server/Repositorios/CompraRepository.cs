using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class CompraRepository : Repository<Compra, int>
    {
        public CompraRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<IEnumerable<Compra>> UltimasCompras (int cant, string cg_mat)
        {
            string xSQL = $"SELECT TOP {cant} * " +
            $"FROM COMPRAS WHERE CG_MAT = '{cg_mat}' AND NUMERO > 0 ORDER BY FE_EMIT DESC";

            return await DbSet.FromSqlRaw(xSQL).ToListAsync();
        }
    }
}
