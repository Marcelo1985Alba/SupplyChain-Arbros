using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Repositorios;

public class ProveedorRepository : Repository<Proveedor, int>
{
    public ProveedorRepository(AppDbContext db) : base(db)
    {
    }
}