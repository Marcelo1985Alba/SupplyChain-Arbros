using System.Threading.Tasks;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class MotivosPresupuestoRepository : Repository<MotivosPresupuesto, int>
{
    public MotivosPresupuestoRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public override async Task Agregar(MotivosPresupuesto entity)
    {
        if (string.IsNullOrEmpty(entity.Motivo)) entity.Motivo = string.Empty;
        await base.Agregar(entity);
    }
}