using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class ModuloRepository : Repository<Modulo, int>
    {
        public ModuloRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }


    }
}
