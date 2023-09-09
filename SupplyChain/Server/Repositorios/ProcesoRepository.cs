using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class ProcesoRepository : Repository<Procesos, int>
{
    public ProcesoRepository(AppDbContext appDb) : base(appDb)
    {
    }
}