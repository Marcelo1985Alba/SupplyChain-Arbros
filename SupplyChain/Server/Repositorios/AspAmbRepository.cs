﻿using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class AspAmbRepository : Repository<AspAmb, int>
    {
        public AspAmbRepository(AppDbContext appDb) : base(appDb)
        {

        }
    }
}
