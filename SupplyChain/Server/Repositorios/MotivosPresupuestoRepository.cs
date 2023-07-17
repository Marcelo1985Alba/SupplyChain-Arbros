using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class MotivosPresupuestoRepository : Repository<MotivosPresupuesto, int>
    {
        public MotivosPresupuestoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        } 

        public override async Task Agregar(MotivosPresupuesto entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.Motivo))
                {
                    entity.Motivo = string.Empty;
                }
                await base.Agregar(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
