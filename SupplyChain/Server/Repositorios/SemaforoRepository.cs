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
        public SemaforoRepository(AppDbContext appDbContext) : base (appDbContext)
        {
        }

        public override async Task Agregar(Semaforo entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.COLOR))
                {
                    entity.COLOR = string.Empty;
                }
                await base.Agregar(entity);
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public SemaforoRepository(AppDbContext appDbContext) : base (appDbContext)
        //{
        //}

        //public async Task<IEnumerable<Semaforo>> GetSemaforos(int id, string color)
        //{
        //    string xSQL = $"SELECT COLOR FROM SEMAFORO";
        //    return await DbSet.FromSqlRaw(xSQL).ToListAsync();
        //}


    }
}
