using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class ProyectosGBPIRepository : Repository<ProyectosGBPI, int>
{
    public ProyectosGBPIRepository(AppDbContext appDb) : base(appDb)
    {
    }
}