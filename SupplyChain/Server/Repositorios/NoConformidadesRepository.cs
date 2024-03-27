using Microsoft.EntityFrameworkCore;
using SupplyChain.Client;
using SupplyChain.Server.Data.Repository;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class NoConformidadesRepository : Repository<NoConformidades, int>
    {
        private readonly AppDbContext appDbContext;
        public NoConformidadesRepository(AppDbContext appDb) : base(appDb) { }

        public async Task<IEnumerable<NoConformidades>> DeleteEvento(int Id)
        {
            string xSQL = $"delete NoConfor where Cg_NoConf={Id}";
            return await DbSet.FromSqlRaw(xSQL).ToListAsync();
            
        }


       

    }
}
