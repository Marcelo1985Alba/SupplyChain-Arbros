using System.Threading.Tasks;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class SemaforoRepository : Repository<Semaforo, int>
{
    public SemaforoRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public override async Task Agregar(Semaforo entity)
    {
        if (string.IsNullOrEmpty(entity.COLOR)) entity.COLOR = string.Empty;
        await base.Agregar(entity);
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