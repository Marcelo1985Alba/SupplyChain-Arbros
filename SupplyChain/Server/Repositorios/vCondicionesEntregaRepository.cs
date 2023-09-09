using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class vCondicionesEntregaRepository : Repository<vCondicionesEntrega, int>
{
    public vCondicionesEntregaRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}