using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class ClienteExternoRepository : Repository<ClienteExterno, string>
{
    public ClienteExternoRepository(AppDbContext appDb) : base(appDb)
    {
    }
}