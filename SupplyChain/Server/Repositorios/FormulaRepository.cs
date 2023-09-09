using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Repositorios;

public class FormulaRepository : Repository<Formula, int>
{
    public FormulaRepository(AppDbContext appContext) : base(appContext)
    {
    }

    public async Task<bool> InsumoEnFormula(string codigo)
    {
        return await DbSet.AnyAsync(f => f.Cg_Prod == codigo || f.Cg_Se == codigo || f.Cg_Mat == codigo);
    }

    public async Task<bool> InsumoEnFormula(List<Producto> insumos)
    {
        var lista = insumos.ConvertAll(p => p.Id);
        return await DbSet.AnyAsync(f =>
            lista.Contains(f.Cg_Prod) || lista.Contains(f.Cg_Se) || lista.Contains(f.Cg_Mat));
    }

    public async Task<bool> InsumoEnFormula(List<string> insumos)
    {
        return await DbSet.AnyAsync(f =>
            insumos.Contains(f.Cg_Prod) || insumos.Contains(f.Cg_Se) || insumos.Contains(f.Cg_Mat));
    }
}