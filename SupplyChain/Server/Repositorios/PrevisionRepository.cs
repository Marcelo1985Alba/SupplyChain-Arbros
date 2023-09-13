using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.Pages.PCP.Prevision;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PrevisionRepository : Repository<PresAnual, int>
    {
        public PrevisionRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<bool> Existe(int id)
        {
            return await base.DbSet.AnyAsync(e => e.Id == id);
        }

        public async Task  AgregarBySP(Producto parametros)
        {
            await Db.Database.ExecuteSqlRawAsync("NET_PCP_PrevisionAgregar '" + parametros.Id.Trim() + "', " +
                                                                          "'" + parametros.DES_PROD.Trim() + "', " +
                                                                          "'" + parametros.UNID + "', " +
                                                                          " " + 1 );
        }
    }
}
