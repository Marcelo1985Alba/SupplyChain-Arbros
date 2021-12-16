using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class FormulaRepository : Repository<Formula, int>
    {
        public FormulaRepository(AppDbContext appContext) : base(appContext)
        {

        }

        public async Task<bool> InsumoEnFormula(string codigo)
        {
            return await base.DbSet.AnyAsync(f=> f.Cg_Prod == codigo || f.Cg_Se == codigo || f.Cg_Mat == codigo);
        }
    }
}
