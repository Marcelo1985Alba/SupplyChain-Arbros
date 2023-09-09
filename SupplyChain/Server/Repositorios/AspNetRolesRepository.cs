using SupplyChain.Server.Data.Repository;

namespace SupplyChain.Server.Repositorios;

public class AspNetRolesRepository : Repository<AspNetRoles, string>
{
    public AspNetRolesRepository(AppDbContext appDb) : base(appDb)
    {
    }
}