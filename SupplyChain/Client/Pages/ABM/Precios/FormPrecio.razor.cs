using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.ABM.Precios;

public class FormPrecioBase : ComponentBase
{
    protected List<Areas> area = new();
    protected bool camposConf = true;
    protected List<Cat> cat = new();
    protected List<Celdas> celda = new();

    protected Dictionary<string, object> HtmlAttributeSubmit = new()
    {
        { "type", "submit" }
    };

    protected List<Lineas> linea = new();
    protected List<Moneda> monedas = new();

    protected SfGrid<Producto> refGridItems;
    protected SfSpinner refSpinnerCli;
    protected bool SpinnerVisible = false;
    protected List<TipoArea> tipoarea = new();
    protected SfToast ToastObj;
    protected List<Unidades> unidades = new();
    [Inject] protected HttpClient Http { get; set; }
    [Inject] public PrecioArticuloService PrecioArticuloService { get; set; }
    [Inject] protected CeldasService CeldasService { get; set; }
    [Inject] protected AreasService AreasService { get; set; }
    [Parameter] public PreciosArticulos PrecioArticulo { get; set; } = new();
    [Parameter] public bool Show { get; set; }
    [Parameter] public EventCallback<PreciosArticulos> OnGuardar { get; set; }
    [Parameter] public EventCallback OnCerrar { get; set; }
    protected bool IsAdd { get; set; }

    protected override async Task OnInitializedAsync()
    {
        unidades = await Http.GetFromJsonAsync<List<Unidades>>("api/unidades");
        monedas = await Http.GetFromJsonAsync<List<Moneda>>("api/Monedas");
        var response = await CeldasService.Get();
        if (!response.Error) celda = response.Response;
        //celda = await Http.GetFromJsonAsync<List<SupplyChain.Celdas>>("api/Celdas");
        var response_2 = await AreasService.Get();
        if (!response_2.Error) area = response_2.Response;
        //area = await Http.GetFromJsonAsync<List<SupplyChain.Areas>>("api/Areas");
        linea = await Http.GetFromJsonAsync<List<Lineas>>("api/Lineas");
        tipoarea = await Http.GetFromJsonAsync<List<TipoArea>>("api/TipoArea");
        cat = await Http.GetFromJsonAsync<List<Cat>>("api/Cat");
    }

    private async Task<bool> Existe()
    {
        var existe = await PrecioArticuloService.Existe(PrecioArticulo.Id);
        return existe;
    }

    protected async Task<bool> Agregar(PreciosArticulos precio)
    {
        if (!await Existe())
        {
            var response = await PrecioArticuloService.Agregar(precio);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                await ToastMensajeError("Error al intentar Guardar el Articulo.");
                return false;
            }

            PrecioArticulo = response.Response;
            PrecioArticulo.GUARDADO = true;
            return true;
        }

        await ToastMensajeError(
            $"El Articulo con codigo {precio.Id} ya existe.\n\rO El tipo de insumo no es permitidio.");


        return false;
    }

    protected async Task<bool> Actualizar(PreciosArticulos precio)
    {
        var response = await PrecioArticuloService.Actualizar(precio.Id, precio);
        if (response.Error)
        {
            await ToastMensajeError("Error al intentar Guardar el producto.");
            return false;
        }

        PrecioArticulo = precio;
        PrecioArticulo.GUARDADO = true;
        return true;
    }

    protected async Task GuardarPrecioArticulo()
    {
        if (PrecioArticulo.ESNUEVO)
            await Agregar(PrecioArticulo);
        else
            await Actualizar(PrecioArticulo);

        if (PrecioArticulo.GUARDADO)
        {
            Show = false;
            await OnGuardar.InvokeAsync(PrecioArticulo);
        }
    }

    public async Task Hide()
    {
        Show = false;
    }

    protected async Task OnCerrarDialog()
    {
        await OnCerrar.InvokeAsync();
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