using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Repositorios;

public class PrevisionRepository : Repository<PresAnual, int>
{
    public PrevisionRepository(AppDbContext db) : base(db)
    {
    }

    public async Task<bool> Existe(int id)
    {
        return await DbSet.AnyAsync(e => e.Id == id);
    }

    public async Task AgregarBySP(Producto parametros)
    {
        await Db.Database.ExecuteSqlRawAsync("NET_PCP_PrevisionAgregar '" + parametros.Id.Trim() + "', " +
                                             "'" + parametros.DES_PROD.Trim() + "', " +
                                             "'" + parametros.UNID + "', " +
                                             " " + 1);
    }
}