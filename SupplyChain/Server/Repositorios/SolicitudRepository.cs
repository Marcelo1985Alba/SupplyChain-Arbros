using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class SolicitudRepository : Repository<Solicitud, int>
    {
        public SolicitudRepository(AppDbContext appDbContext) : base (appDbContext)
        {

        }

        public async Task<bool> Existe(int id)
        {
            return await DbSet.AnyAsync(e => e.Id == id);
        }

        public async Task<List<vSolicitudes>> ObtenerTodosFromVista()
        {
            return await Db.vSolicitudes.Where(s => !s.TienePresupuesto).ToListAsync();
        }

        public async Task AsignarClientByCuit(string cuit, Solicitud solicitud)
        {
            cuit = cuit.Insert(2, "-");
            cuit = cuit.Insert(cuit.Length - 1, "-");
            var cliente = await Db.Cliente.FirstOrDefaultAsync(c=> c.CUIT.Trim() == cuit);
            if (cliente != null)
            {
                solicitud.CG_CLI = cliente.CG_CLI;
            }
        }
    }
}
