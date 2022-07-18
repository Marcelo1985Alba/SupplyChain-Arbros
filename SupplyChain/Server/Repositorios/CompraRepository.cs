using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class CompraRepository : Repository<Compra, int>
    {
        public CompraRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }
    }
}
