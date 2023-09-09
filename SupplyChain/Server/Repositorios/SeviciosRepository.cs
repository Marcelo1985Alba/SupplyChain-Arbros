using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Enum;

namespace SupplyChain.Server.Repositorios;

public class ServiciosRepository : Repository<Service, int>
{
    public ServiciosRepository(AppDbContext db) : base(db)
    {
    }

    public async Task<List<Service>> GetByFilter(TipoFiltro tipoFiltro = TipoFiltro.Todos)
    {
        if (tipoFiltro == TipoFiltro.Pendientes)
            return await Obtener(s => string.IsNullOrEmpty(s.REMITO)).ToListAsync();
        //return await base.Obtener(s => string.IsNullOrEmpty(s.PEDIDO)).ToListAsync();
        if (tipoFiltro == TipoFiltro.NoPendientes)
        {
            //return await base.Obtener(s => s.PEDIDO > 0).ToListAsync();
        }

        return await base.ObtenerTodos();
    }
}