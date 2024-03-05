using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PresupuestoDetalleRepository : Repository<PresupuestoDetalle,int>
    {
        public PresupuestoDetalleRepository(AppDbContext appDbContext) : base(appDbContext) { }


       
    }
}
