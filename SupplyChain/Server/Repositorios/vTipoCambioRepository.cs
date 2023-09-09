using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class vTipoCambioRepository : Repository<vTipoCambio, int>
{
    public vTipoCambioRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}