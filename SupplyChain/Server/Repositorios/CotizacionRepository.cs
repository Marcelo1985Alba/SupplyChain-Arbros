using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class CotizacionRepository : Repository<Cotizaciones, int>
    {
        public CotizacionRepository(AppDbContext appContext) : base(appContext)
        {

        }
    }
}
