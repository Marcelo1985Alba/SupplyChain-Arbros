using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class ProveedorRepository : Repository<Proveedor, int>
    {
        public ProveedorRepository(AppDbContext db) : base(db)
        {
        }
    }
}
