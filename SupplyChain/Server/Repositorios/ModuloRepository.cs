using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class ModuloRepository : Repository<Modulo, int>
{
    public ModuloRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}