using SupplyChain.Server.Data.Repository;
using Syncfusion.Pdf.Lists;

namespace SupplyChain.Server.Repositorios
{
    public class ProcesoRepository: Repository<Procesos, int>
    {
        public ProcesoRepository(AppDbContext appDb) : base(appDb)
        {

        }
    }
}
