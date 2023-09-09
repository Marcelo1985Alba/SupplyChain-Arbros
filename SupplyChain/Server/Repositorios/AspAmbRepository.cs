using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class AspAmbRepository : Repository<AspAmb, int>
{
    public AspAmbRepository(AppDbContext appDb) : base(appDb)
    {
    }
}