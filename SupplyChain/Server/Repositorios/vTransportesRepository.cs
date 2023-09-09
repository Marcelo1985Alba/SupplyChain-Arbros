using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class vTransportesRepository : Repository<vTransporte, int>
{
    public vTransportesRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}