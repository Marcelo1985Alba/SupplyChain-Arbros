using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class NoConformidadesRepository : Repository<NoConformidades, int>
    {
        public NoConformidadesRepository(AppDbContext appDb) : base(appDb) { }

        //public async Task<IEnumerable<NoConformidades>> DeleteEvento(NoConformidades noConf)
        //{
        //    string xSQL = $"delete NoConfor where Cg_NoConf={noConf.Id}";
        //    await DbSet.FromSqlRaw(xSQL).ToListAsync();
        //    return Enumerable.Empty<NoConformidades>();
        //}


    }
}
