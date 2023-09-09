using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Repositorios;

public class ProductoRepository : Repository<Producto, string>
{
    public ProductoRepository(AppDbContext appDb) : base(appDb)
    {
    }
}