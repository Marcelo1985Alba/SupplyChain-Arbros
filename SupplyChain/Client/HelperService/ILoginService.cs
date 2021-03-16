using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public interface ILoginService
    {
        Task Login(Usuarios Usuario);
        Task Logout();
    }
}
