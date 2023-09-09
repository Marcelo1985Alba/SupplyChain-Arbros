using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class vCondicionesPagoRepository : Repository<vCondicionesPago, int>
{
    public vCondicionesPagoRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}