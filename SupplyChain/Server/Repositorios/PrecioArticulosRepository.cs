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
    public class PrecioArticulosRepository : Repository<PreciosArticulos, string>
    {
        public PrecioArticulosRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }


        internal async Task<IEnumerable<PreciosArticulos>> Search(string codigo, string descripcion)
        {
            IQueryable<PreciosArticulos> query = DbSet.AsQueryable();

            if (codigo != "VACIO")
            {
                query = query.Where(p => p.Id.Contains(codigo));
            }

            if (descripcion != "VACIO")
            {
                query = query.Where(p => p.Descripcion.Contains(descripcion));
            }

            return await query.ToListAsync();

        }
    }
}
