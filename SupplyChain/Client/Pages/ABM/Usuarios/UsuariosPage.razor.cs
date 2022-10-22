using Microsoft.AspNetCore.Components;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.Usuarios
{
    public class UsuariosPageBase : ComponentBase
    {
        [Inject] RepositoryHttp.IRepositoryHttp Http { get; set; }


        protected List<ApplicationUser> Usuarios = new();
            
        protected async override Task OnInitializedAsync()
        {
            await Http.GetFromJsonAsync<List<ApplicationUser>>("api/Cuentas/Usuarios");
        }
    }
}
