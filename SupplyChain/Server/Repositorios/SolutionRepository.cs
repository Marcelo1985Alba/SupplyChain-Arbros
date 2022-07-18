using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios
{
    public class SolutionRepository : Repository<Solution, string>
    {
        public SolutionRepository(AppDbContext db) : base(db)
        {
        }
    }
}
