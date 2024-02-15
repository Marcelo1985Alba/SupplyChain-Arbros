using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SupplyChain.Server.Repositorios
{
    public class ProcedimientosRepository : Repository<Operaciones, int>
    {
        public ProcedimientosRepository(AppDbContext appDb) : base(appDb)
        {

        }

        internal async Task<IEnumerable<Operaciones>> Search(int idProc, string Cg_Prod)
        {
            IQueryable<Operaciones> query = DbSet.AsQueryable().Where(p => p.CG_ORDEN == 1 || p.CG_ORDEN == 3);
            if (idProc !=0)
            {
                query = query.Where(p => p.Id.Equals(idProc));
            }
            if (Cg_Prod != "VACIO")
            {
                query = query.Where(p => p.CG_PROD.Contains(Cg_Prod));
            }
            //return await query.ToListAsync();
            var lista = await query.ToListAsync();
            return lista;
        }
    }
}

