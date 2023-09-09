using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Kanban;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Pages.Modelos;

public class EstadoPageBase : ComponentBase
{
    protected Query CardQuery = new();

    public string Cliente;
    public bool Disabled = false;
    public bool Enabled = true;
    protected SfGrid<PedCli> Grid;

    protected List<PedCli> Pedclis = new();

    //protected List<Pedidos> Pedidoss = new List<Pedidos>();
    //protected List<Programa> Programas = new List<Programa>();
    protected string PriorityValue = "None";
    protected SfKanban<PedCli> refKanban;
    protected string SearchValue = string.Empty;
    protected SfSpinner spinnerRef;
    protected string StatusValue = "None";
    protected SfToast ToastObj;
    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    protected bool SpinnerVisible { get; set; }
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

    protected void onDragStart(DragEventArgs<PedCli> args)
    {
        //status = args.Data[0].Status;
        Cliente = args.Data[0].DES_CLI;
    }

    protected async Task onDragStop(DragEventArgs<PedCli> args)
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
            var Nuevo = args.Data[0];
            response = await Http.PutAsJsonAsync($"api/Pedcli/{Nuevo.PEDIDO}", Nuevo);
            if (!response.IsSuccessStatusCode)
                await ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = $"Error al actualizar pedido {Nuevo.PEDIDO}",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
        }
    }

    protected void OnCardSearch(ChangeEventArgs args)
    {
        var searchString = (string)args.Value;
        if (searchString == string.Empty)
            CardQuery = new Query();
        else
            CardQuery = new Query().Search(searchString, new List<string> { "PEDIDO", "DES_ART", "DES_CLI" },
                "contains", true);
    }

    protected async Task OnActionBegin(Syncfusion.Blazor.Kanban.ActionEventArgs<PedCli> args)
    {
        if (args.RequestType == "cardRemove")
        {
            await ToastObj.ShowAsync
            (new ToastModel
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
            await ToastObj.ShowAsync(new ToastModel
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
                if (row.CONFIRMADO) Pedclis = Pedclis.Where(p => p.PEDIDO != row.PEDIDO).ToList();

                await ToastObj.ShowAsync(new ToastModel
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
                await ToastObj.ShowAsync(new ToastModel
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