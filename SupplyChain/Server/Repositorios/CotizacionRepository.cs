using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class CotizacionRepository : Repository<Cotizaciones, int>
{
    public CotizacionRepository(AppDbContext appContext) : base(appContext)
    {
    }
}