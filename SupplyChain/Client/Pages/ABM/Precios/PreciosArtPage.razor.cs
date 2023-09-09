using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Action = Syncfusion.Blazor.Grids.Action;

namespace SupplyChain.Client.Pages.ABM.Precios;

public class PreciosArtPageBase : ComponentBase
{
    protected const string APPNAME = "grdPreciosArtABM";
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<PreciosArticulos> Grid;
    public bool isAdding;

    protected List<Moneda> monedas = new();
    public PreciosArticulos preciosArticuloSeleccionado = new();


    protected List<PreciosArticulos> preciosArts = new();
    protected SfSpinner refSpinner;
    public bool SpinnerVisible;
    protected string state;
    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        "Add",
        "Edit",
        "Delete",
        new ItemModel { Text = "Copia", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
        new ItemModel
            { Text = "Exportar Excel", TooltipText = "Exportar a excel", PrefixIcon = "e-export", Id = "exportar" }
    };

    [Inject] protected PrecioArticuloService PrecioArticuloService { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public HttpClient Http { get; set; }
    protected bool popupFormVisible { get; set; }
    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Precios de Articulos";

        SpinnerVisible = true;
        await GetPrecioArticulos();

        SpinnerVisible = false;
    }

    protected async Task OnActionBeginHandler(ActionEventArgs<PreciosArticulos> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
            preciosArticuloSeleccionado = new PreciosArticulos();
            preciosArticuloSeleccionado.ESNUEVO = true;
        }

        if (args.RequestType == Action.BeginEdit)
        {
            preciosArticuloSeleccionado = args.Data;
            preciosArticuloSeleccionado.ESNUEVO = false;
        }

        if (args.RequestType == Action.Grouping
            || args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.Reorder
            || args.RequestType == Action.Sorting
           )
        {
            //VisibleProperty = true;
            Grid.PreventRender();
            Grid.Refresh();

            state = await Grid.GetPersistData();
            await Grid.AutoFitColumnsAsync();
            await Grid.RefreshColumns();
            await Grid.RefreshHeader();
        }
    }

    protected async Task OnActionCompleteHandler(ActionEventArgs<PreciosArticulos> args)
    {
        if (args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
        }
    }

    protected async Task GetPrecioArticulos()
    {
        var respose = await PrecioArticuloService.Get();
        if (respose.Error)
        {
            Console.WriteLine(await respose.HttpResponseMessage.Content.ReadAsStringAsync());
            await ToastMensajeError();
        }
        else
        {
            preciosArts = respose.Response.OrderBy(p => p.Id).ToList();
            monedas = await Http.GetFromJsonAsync<List<Moneda>>("api/Monedas");
        }
    }

    protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
    {
        await Grid.SetPersistData(vistasGrillas.Layout);
    }

    protected async Task OnReiniciarGrilla()
    {
        await Grid.ResetPersistData();
    }

    protected void OnCerraDialog()
    {
        popupFormVisible = false;
    }

    protected async Task Guardar(PreciosArticulos precio)
    {
        if (precio.GUARDADO)
        {
            await ToastMensajeExito();
            popupFormVisible = false;
            if (precio.ESNUEVO)
            {
                preciosArts.Add(precio);
            }
            else
            {
                //actualizar datos sin ir a la base de datos
                var prec = preciosArts.FirstOrDefault(p => p.Id == precio.Id);
                prec.Descripcion = precio.Descripcion;
                prec.Construccion = precio.Construccion;
                prec.Marca = precio.Marca;
                prec.Moneda = precio.Moneda;
                prec.Precio = precio.Precio;
            }

            Grid.Refresh();
        }
        else
        {
            await ToastMensajeError();
        }
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Edit")
        {
            if (Grid.SelectedRecords.Count < 2)
            {
                foreach (var selectedRecord in Grid.SelectedRecords)
                {
                    preciosArticuloSeleccionado.Id = selectedRecord.Id;
                    preciosArticuloSeleccionado.Descripcion = selectedRecord.Descripcion;
                    preciosArticuloSeleccionado.Precio = selectedRecord.Precio;
                    preciosArticuloSeleccionado.Moneda = selectedRecord.Moneda;
                    preciosArticuloSeleccionado.Marca = selectedRecord.Marca;
                    preciosArticuloSeleccionado.Construccion = selectedRecord.Construccion;
                }

                isAdding = false;
                //IsVisible = true;
            }
            else
            {
                await ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Solo se puede editar un item",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
        }

        if (args.Item.Text == "Copia" || args.Item.Text == "Add")
        {
            if (Grid.SelectedRecords.Count == 1)
            {
                preciosArticuloSeleccionado = new PreciosArticulos();

                foreach (var selectedRecord in Grid.SelectedRecords)
                {
                    var isConfirmed =
                        await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el producto?");
                    if (isConfirmed)
                    {
                        preciosArticuloSeleccionado.Descripcion = selectedRecord.Descripcion;
                        preciosArticuloSeleccionado.Precio = selectedRecord.Precio;
                        preciosArticuloSeleccionado.Moneda = selectedRecord.Moneda;
                        preciosArticuloSeleccionado.Marca = selectedRecord.Marca;
                        preciosArticuloSeleccionado.Construccion = selectedRecord.Construccion;
                    }
                }

                popupFormVisible = true;
                isAdding = true;
                preciosArticuloSeleccionado.ESNUEVO = true;
            }
            else
            {
                if (args.Item.Text == "Add")
                {
                    await Grid.ClearSelection();
                    //Grid.Refresh();
                    popupFormVisible = true;
                    isAdding = true;
                    preciosArticuloSeleccionado.ESNUEVO = true;
                }
                else
                {
                    await ToastObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "Solo se puede copiar un item",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
            }
        }

        if (args.Item.Text == "Exportar Excel") await Grid.ExcelExport();

        if (args.Item.Text == "Eliminar")
            if ((await Grid.GetSelectedRecordsAsync()).Count > 0)
                foreach (var selectedRecord in Grid.SelectedRecords)
                {
                    var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                        "Seguro de que desea eliminar el precio de articulo?");
                    if (isConfirmed) await Http.DeleteAsync($"api/PreciosArt/{selectedRecord.Id}");
                }
    }

    private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
    {
        await ToastObj.Show(new ToastModel
        {
            Title = "EXITO!",
            Content = content,
            CssClass = "e-toast-success",
            Icon = "e-success toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
    }

    private async Task ToastMensajeError(string content = "Ocurrio un Error.")
    {
        await ToastObj.Show(new ToastModel
        {
            Title = "Error!",
            Content = content,
            CssClass = "e-toast-warning",
            Icon = "e-warning toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
    }
}