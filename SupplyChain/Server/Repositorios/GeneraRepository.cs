using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class GeneraRepository : Repository<Genera, string>
    {
        public GeneraRepository(AppDbContext appDbContext) : base (appDbContext)
        {
            appDbContext.Database.SetCommandTimeout(180);
        }

        

        public async Task Reserva(string campo)
        {
            var sp = $"Exec N_Genera 1, '{campo}', 'R', 0, '', 0, 0";
            await Db.Database.ExecuteSqlRawAsync(sp);
        }

        public async Task Libera(string campo)
        {
            var sp = $"Exec N_Genera 1, '{campo}', 'L', 0, '', 0, 0";
            await Db.Database.ExecuteSqlRawAsync(sp);
        }
    }
}
