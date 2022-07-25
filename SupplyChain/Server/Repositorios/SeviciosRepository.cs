using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class ServiciosRepository : Repository<Service, int>
    {
        public ServiciosRepository(AppDbContext db) : base(db)
        {
        }
    }
}
