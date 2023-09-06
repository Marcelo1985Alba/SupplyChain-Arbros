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
    public class ProductoRepository : Repository<Producto, string>
    {
        public ProductoRepository(AppDbContext appDb) : base(appDb)
        {

        }

        internal async Task<IEnumerable<Producto>> Search(string idProd, string Des_prod)
        {
            IQueryable<Producto> query = DbSet.AsQueryable().Where(p=> p.CG_ORDEN==1 || p.CG_ORDEN==3);
            if (idProd != "VACIO")
            {
                query = query.Where(p => p.Id.Contains(idProd));
            }
            if (Des_prod != "VACIO")
            {
                query = query.Where(p => p.DES_PROD.Contains(Des_prod));
            }
            //return await query.ToListAsync();
            var lista = await query.ToListAsync();
            return lista;
        }
    }
}
