using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class MantCeldasRepository : Repository<MantCeldas, int>
{
    public MantCeldasRepository(AppDbContext appDb) : base(appDb)
    {
    }
}