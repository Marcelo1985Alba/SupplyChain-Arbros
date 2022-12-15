using SupplyChain.Server.Data.Repository;
using Syncfusion.Pdf.Lists;

namespace SupplyChain.Server.Repositorios
{
    public class CargaValoresRepository : Repository<Valores,int>
    {
        public CargaValoresRepository(AppDbContext appDb) : base(appDb)
        {

        }
    }
}
