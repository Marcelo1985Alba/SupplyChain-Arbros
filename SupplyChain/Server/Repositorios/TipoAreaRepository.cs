using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class TipoAreaRepository : Repository<TipoArea, int>
{
    public TipoAreaRepository(AppDbContext appDb) : base(appDb)
    {
    }
}