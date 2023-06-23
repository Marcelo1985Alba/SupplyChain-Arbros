using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class SemaforoRepository : Repository<Semaforo, int>
    {
        public SemaforoRepository(AppDbContext db) : base(db)
        {
        }
        //public async Task<IEnumerable<string>> AsignaColor(vPresupuestos entity)
        //{
        //    try
        //    {
        //        foreach (var item in entity.Items)
        //        {
        //            if (item.COLOR == " ")
        //            {
        //                await Db.Database.ExecuteSqlRawAsync($"UPDATE vPresupuestos SET COLOR ={item.COLOR}  WHERE ID ={item.Id}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"ERROR: {ex.Message}");
        //    }
        //}

        //public async Task<IEnumerable<Semaforo>> ColorSemaforo()
        //{
        //    string xSQL = string.Format("SELECT COLOR FROM SEMAFORO");
        //    return await base.DbSet.FromSqlRaw(xSQL).ToListAsync();
        //}
        
    }
}
