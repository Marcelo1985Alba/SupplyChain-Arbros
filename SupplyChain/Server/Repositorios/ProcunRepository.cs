using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class ProcunRepository : Repository<Procun, decimal>
    {
        public ProcunRepository(AppDbContext appDb) : base(appDb)
        {

        }

        internal async Task<IEnumerable<Procun>> Search(string idProd, string Des_Prod)
        {
            IQueryable<Procun> query = DbSet.AsQueryable();
            if(idProd != "VACIO")
            {
                query = query.Where(p => p.CG_PROD.Contains(idProd));
            }
            if(Des_Prod != "VACIO")
            {
                query = query.Where(p=>p.Des_Prod.Contains(Des_Prod));
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<vProcun>> ObtenerProcun()
        {
            string xSQL = string.Format("select * from vprocun");

            return await base.Db.vProcun.FromSqlRaw(xSQL).ToListAsync();

        }

       


    }
}
