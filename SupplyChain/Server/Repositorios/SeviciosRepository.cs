using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class ServiciosRepository : Repository<Service, int>
    {
        public ServiciosRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<List<Service>> GetByFilter(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            if (tipoFiltro == TipoFiltro.Pendientes)
            {
                return await base.Obtener(s => string.IsNullOrEmpty(s.REMITO)).ToListAsync();
                //return await base.Obtener(s => string.IsNullOrEmpty(s.PEDIDO)).ToListAsync();
            }

            if (tipoFiltro == TipoFiltro.NoPendientes)
            {
                //return await base.Obtener(s => s.PEDIDO > 0).ToListAsync();
            }

            return await base.ObtenerTodos();
        }
    }
}
