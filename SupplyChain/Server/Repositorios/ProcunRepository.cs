using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class ProcunRepository : Repository<Procun, decimal>
{
    public ProcunRepository(AppDbContext appDb) : base(appDb)
    {
    }
}