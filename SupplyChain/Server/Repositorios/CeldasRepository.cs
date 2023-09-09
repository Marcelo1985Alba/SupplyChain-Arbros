using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Repositorios;

public class CeldasRepository : Repository<Celdas, string>
{
    public CeldasRepository(AppDbContext appDb) : base(appDb)
    {
    }
}