using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class ProyectosGBPIRepository : Repository<ProyectosGBPI, int>
    {
        public ProyectosGBPIRepository(AppDbContext appDb) : base(appDb)
        {

        }
    }
}
