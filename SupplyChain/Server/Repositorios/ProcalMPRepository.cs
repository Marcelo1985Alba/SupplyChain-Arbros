using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios
{
    public class ProcalMPRepository : Repository<ProcalsMP, int>
    {
        public ProcalMPRepository(AppDbContext appDb) : base (appDb)
        {

        }
    }
}
