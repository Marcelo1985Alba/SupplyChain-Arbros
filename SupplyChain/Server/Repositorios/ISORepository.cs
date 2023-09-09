using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class ISORepository : Repository<ISO, int>
{
    public ISORepository(AppDbContext appDb) : base(appDb)
    {
    }
}