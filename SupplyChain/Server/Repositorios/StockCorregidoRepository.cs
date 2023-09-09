using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

public class StockCorregidoRepository : Repository<StockCorregido, string>
{
    public StockCorregidoRepository(AppDbContext db) : base(db)
    {
    }
}