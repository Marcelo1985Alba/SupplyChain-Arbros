using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class AreasRepository : Repository<Areas, int>
{
    public AreasRepository(AppDbContext appDb) : base(appDb)
    {
    }
}