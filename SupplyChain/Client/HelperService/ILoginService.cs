using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public interface ILoginService
    {
        Task Login(string Usuario);
        Task Logout();
    }
}
