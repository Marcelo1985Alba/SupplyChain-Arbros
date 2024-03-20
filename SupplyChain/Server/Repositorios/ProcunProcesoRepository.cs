using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SupplyChain.Server.Repositorios
{
    public class ProcunProcesoRepository : Repository<Protab, string>
    {
        public ProcunProcesoRepository(AppDbContext appDb) : base(appDb)
        {

        }

       

        internal async Task<IEnumerable<Protab>> Search(string id, string PROCESO)
        {
            IQueryable<Protab> query = DbSet.AsQueryable();
            if (id != "VACIO")
            {
                query = query.Where(p => p.Id.Equals(id));
            }
            if (PROCESO != "VACIO")
            {
                query = query.Where(p => p.Id.Contains(PROCESO));
            }
            //return await query.ToListAsync();
            var lista = await query.ToListAsync();
            return lista;
        }
    }
}

