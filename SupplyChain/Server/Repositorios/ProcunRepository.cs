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



        //public async Task<IEnumerable<Procun>> ActualizaCelda(decimal id, string cg_celda)
        //{
        //    string xSQL = $"UPDATE PROCUN SET CG_PROD='{cg_celda}' WHERE ID ={id}";
        //    await base.Database.ExecuteSqlRawAsync(xSQL);

        //    return await DbSet.Where(p=>p.Id == id).ToListAsync();
        //}
        //public async Task<IEnumerable<Procun>> ActualizaProceso(decimal id, string proceso)
        //{
        //    string xSQL = $"UPDATE PROCUN SET PROCESO='{proceso}' WHERE ID ={id}";
        //    await base.Database.ExecuteSqlRawAsync(xSQL);

        //    return await DbSet.Where(p => p.Id == id).ToListAsync();
        //}
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
    }
}
