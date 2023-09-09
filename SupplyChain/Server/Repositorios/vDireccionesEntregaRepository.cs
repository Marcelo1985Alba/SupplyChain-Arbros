using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class vDireccionesEntregaRepository : Repository<vDireccionesEntrega, int>
{
    public vDireccionesEntregaRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}