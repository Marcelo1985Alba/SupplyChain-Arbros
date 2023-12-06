using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios
{
    public class CampoComRepository : Repository<CampoComodin,int>
    {

        public CampoComRepository(AppDbContext appDb) : base(appDb) { }


    }
}
