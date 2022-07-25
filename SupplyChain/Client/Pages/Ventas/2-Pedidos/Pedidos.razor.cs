using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._2_Pedidos
{
    public class PedidosBase : ComponentBase
    {
        [Inject] public PedCliService PedCliService { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfGrid<PedCli> refGrid;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        protected List<PedCli> pedidos = new();
        protected bool SpinnerVisible = false;

        protected List<Object> Toolbaritems = new()
        {
            "Search",
            //new ItemModel { Text = "Add", TooltipText = "Agregar un nuevo Presupuesto", PrefixIcon = "e-add", Id = "Add" },
            "Add",
            "Edit",
            "Delete",
            "Print",
            //new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
            "ExcelExport",
            new ItemModel { Text = "", TooltipText = "Actualizar Grilla", PrefixIcon = "e-refresh", Id = "refresh" },
        };
        protected async override Task OnInitializedAsync()
        {
            SpinnerVisible = true;
            await GetPedidos();
            await refGrid.AutoFitColumnsAsync();
            SpinnerVisible = false;
        }

        protected async Task GetPedidos()
        {
            var response = await PedCliService.Get();
            if (response.Error)
            {

            }
            else
            {
                pedidos = response.Response;
            }
        }

    }
}
