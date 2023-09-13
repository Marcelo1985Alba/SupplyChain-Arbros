using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class NotificacionRepository : Repository<NotificacionSubscripcion, int>
    {
        public NotificacionRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }


        public async Task RemoveRange(NotificacionSubscripcion[] notificacionSubscripcions)
        {
            Db.RemoveRange(notificacionSubscripcions);
            await Db.SaveChangesAsync();
        }
    }
}
