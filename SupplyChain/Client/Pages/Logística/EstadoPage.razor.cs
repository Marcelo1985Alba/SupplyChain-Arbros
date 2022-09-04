using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Kanban;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Pages.Modelos
{
    public class EstadoPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        protected SfKanban<PedCli> refKanban;
        protected SfGrid<PedCli> Grid;
        protected SfToast ToastObj;
        public bool Enabled = true;
        public bool Disabled = false;
        protected SfSpinner spinnerRef;
        protected bool SpinnerVisible { get; set; } = false;
        protected List<PedCli> Pedclis = new List<PedCli>();
        //protected List<Pedidos> Pedidoss = new List<Pedidos>();
        //protected List<Programa> Programas = new List<Programa>();
        protected string PriorityValue = "None";
        protected string StatusValue = "None";
        protected string SearchValue = string.Empty;
        protected Query CardQuery = new();
        [CascadingParameter] public MainLayout Layout { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SpinnerVisible = true;
            Layout.Titulo = "Logistica";
            Pedclis = await Http.GetFromJsonAsync<List<PedCli>>("api/PedCli/ObtenerPedCliPedidos");
            //Pedidoss = await Http.GetFromJsonAsync<List<Pedidos>>("api/Pedidos");
            //Programas = await Http.GetFromJsonAsync<List<Programa>>("api/Programa");
            SpinnerVisible = false;
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

        protected async Task OnActionBegin(Syncfusion.Blazor.Kanban.ActionEventArgs<PedCli> args)
        {
            if (args.RequestType == "cardRemove")
            {
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "ADVERTENCIA!",
                    Content = $"El pedido {args.ChangedRecords.ToList()[0].PEDIDO} no se puede eliminar desde este modulo.",
                    CssClass = "e-toast-warning",
                    Icon = "e-warning toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
                args.Cancel = true;

            }
        }

        protected async Task OnActionComplete(Syncfusion.Blazor.Kanban.ActionEventArgs<PedCli> args)
        {
            if (args.RequestType == "cardRemove")
            {
                args.Cancel = true;
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "ADVERTENCIA!",
                    Content = $"El pedido {args.DeletedRecords.ToList()[0].PEDIDO} no se puede eliminar desde este modulo.",
                    CssClass = "e-toast-warning",
                    Icon = "e-warning toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }

            if (args.RequestType == "cardChange")
            {
                var row = args.ChangedRecords.ToList()[0];
                var response = await Http.PutAsJsonAsync("api/Pedidos/FromPedCli", row);

                if (response.IsSuccessStatusCode)
                {
                    if (row.CONFIRMADO)
                    {
                        Pedclis = Pedclis.Where(p => p.PEDIDO != row.PEDIDO).ToList();
                    }
                    
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = $"Pedido {row.PEDIDO} Guardado Correctamente.",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });

                    await refKanban.RefreshAsync();
                }
                else
                {
                    args.Cancel = true;

                    Console.WriteLine(response.Content.ReadAsStringAsync());
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = $"Error al guardar pedido {row.PEDIDO}",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }

            }

        }
    }
}
