using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.ISOP
{
    public class GraphicIso : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public ISOService isoService { get; set; }

        protected List<ISO> isos = new();
        protected SfSpinner refSpinner;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        [Parameter] public bool Show { get; set; } = false;

        protected async override Task OnInitializedAsync()
        {
            var response = await isoService.Get();
            if (!response.Error)
            {
                isos = response.Response;
            }
        }


    }
}
