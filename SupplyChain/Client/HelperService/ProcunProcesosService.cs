using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService
{
    public class ProcunProcesosService : BaseService<ProcunProcesos, int>
    {
        private const string API = "api/ProcunProcesos";

        public ProcunProcesosService(IRepositoryHttp httpClient) : base(httpClient, API)
        {

        }


    }
}
