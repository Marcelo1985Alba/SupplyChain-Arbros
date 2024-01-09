using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared.BuscadorProducto;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.ProcunP
{
    public class FormProcun2Base : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected ProcunService ProcunService { get; set; }
        [Inject] protected ProductoService ProductoService { get; set; }
        [Inject] protected AreasService AreasService { get; set; }
        [Inject] protected LineasService LineasService { get; set; }
        [Inject] protected CeldasService CeldasService { get; set; }
        [Parameter] public Procun procuns { get; set; } = new(); 
        [Parameter] public Producto prod { get; set; } = new();
        [Parameter] public bool Show { get; set; } = new();
        [Parameter] public EventCallback<Procun> OnGuardar { get; set; } = new();
        [Parameter] public EventCallback<Procun> OnEliminar { get; set; } = new();
        [Parameter] public EventCallback OnCerrar { get; set; } = new();
        [Parameter] public Producto producto { get; set; } = new Producto();
        protected bool popupBuscarVisibleProducto { get; set; } = false;
        protected ProductoDialog refProductoDialog;
        protected SfGrid<Procun> refGridItems;
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            {"type","submit" },
            {"form","form-procun" }
        };
        protected bool IsAdd {  get; set; }
        protected List<Producto> productos = new();
        protected List<Areas> areas= new();
        protected List<Lineas> lineas = new();
        protected List<Celdas> celdas= new();
        protected List<ProcunProcesos> procunProcesos = new();

        protected SfSpinner refSpinner;

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }


        protected async Task OnInitializedAsync()
        {
            var response = await ProductoService.Get();
            if (!response.Error)
            {
                productos = response.Response;
            }
            var response2 = await AreasService.Get();
            if ((!response2.Error)
            {
                areas= response2.Response; 
            }
            var response3= await LineasService.Get();
            if (!response3.Error)
            {
                lineas = response3.Response;
            }
            var response4= await CeldasService.Get();
            if (!response4.Error)
            {
                celdas = response4.Response;
            }
            procunProcesos = await Http.GetFromJsonAsync<List<ProcunProcesos>>("api/ProcunProcesos");

        }
    }
}
