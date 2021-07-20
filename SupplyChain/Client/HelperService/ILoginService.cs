using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public interface ILoginService
    {
        public Usuarios UsuarioLogin { get; set; }
        Task Login(Usuarios Usuario);
        Task Logout();
    }
}
