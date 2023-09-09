using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class UnidadesRepository : Repository<Unidades, string>
{
    public UnidadesRepository(AppDbContext appDb) : base(appDb)
    {
    }
}