using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class ClienteRepository : Repository<Cliente, int>
{
    public ClienteRepository(AppDbContext appDb) : base(appDb)
    {
    }
}