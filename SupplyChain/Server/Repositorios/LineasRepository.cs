using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class LineasRepository : Repository<Lineas, int>
{
    public LineasRepository(AppDbContext appDb) : base(appDb)
    {
    }
}