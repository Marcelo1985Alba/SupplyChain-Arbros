using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Pages.Modelos
{
    public class EstadoPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        protected SfGrid<PedCli> Grid;
        public bool Enabled = true;
        public bool Disabled = false;

        protected List<PedCli> Pedclis = new List<PedCli>();
        //protected List<Pedidos> Pedidoss = new List<Pedidos>();
        //protected List<Programa> Programas = new List<Programa>();
        protected string PriorityValue = "None";
        protected string StatusValue = "None";
        protected string SearchValue = string.Empty;
        protected Query CardQuery = new Query();
        [CascadingParameter] public MainLayout Layout { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Layout.Titulo = "Logistica";
            Pedclis = await Http.GetFromJsonAsync<List<PedCli>>("api/PedCli");
            //Pedidoss = await Http.GetFromJsonAsync<List<Pedidos>>("api/Pedidos");
            //Programas = await Http.GetFromJsonAsync<List<Programa>>("api/Programa");

            await base.OnInitializedAsync();
        }

        public string Cliente;
        
        protected void onDragStart(Syncfusion.Blazor.Kanban.DragEventArgs<PedCli> args)
        {
            //status = args.Data[0].Status;
            Cliente = args.Data[0].DES_CLI;
        }
        protected async Task onDragStop(Syncfusion.Blazor.Kanban.DragEventArgs<PedCli> args)
        {
            HttpResponseMessage response;
            if (args.Data[0].DES_CLI != Cliente)
            {
                // Preventing the drag action between the columns
                args.Cancel = true;
                await JsRuntime.InvokeAsync<bool>("confirm", "No es posible cambiar el cliente");
            }
            else
            {
                PedCli Nuevo = args.Data[0];
                response = await Http.PutAsJsonAsync($"api/Pedcli/{Nuevo.PEDIDO}", Nuevo);
            }
        }

        protected void OnCardSearch(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            string searchString = (string)args.Value;
            if (searchString == string.Empty)
            {
                CardQuery = new Query();
            }
            else
            {
                CardQuery = new Query().Search(searchString, new List<string>() { "PEDIDO", "DES_ART", "DES_CLI" }, "contains", true);
            }
        }
    }
}
