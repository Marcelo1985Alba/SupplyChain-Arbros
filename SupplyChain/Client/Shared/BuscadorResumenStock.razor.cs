using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.Grids;

namespace SupplyChain.Client.Shared;

public class BuscadorResumenStockBase : ComponentBase
{
    protected BuscadorEmergente<vResumenStock> Buscador;
    protected string[] ColumnasBuscador = { "CG_ART", "CG_DEP", "DESPACHO", "SERIE", "LOTE" };
    protected vResumenStock[] ItemsABuscar;
    protected bool PopupBuscadorVisible;
    protected string TituloBuscador = "Listado Insumos con Stock";
    [Inject] private HttpClient Http { get; set; }
    [Parameter] public string TextoButon { get; set; } = "";
    [Parameter] public bool EsSoloLectura { get; set; }
    [Parameter] public bool MostraBotonBusqueda { get; set; }
    [Parameter] public bool HabilitaBotonBusqueda { get; set; }
    [Parameter] public bool MostrarSpin { get; set; } = true;
    [Parameter] public EventCallback<vResumenStock> OnRSSeleccionada { get; set; }
    [Parameter] public SelectionType TipoSeleccion { get; set; } = SelectionType.Single;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> HtmlAtrr { get; set; } = new() { { "type", "button" } };

    protected async Task BuscarStock()
    {
        PopupBuscadorVisible = true;
        await Buscador.ShowAsync();
        ItemsABuscar = await Http.GetFromJsonAsync<vResumenStock[]>("api/ResumenStock");
        await InvokeAsync(StateHasChanged);
    }

    protected async Task EnviarObjetoSeleccionado(vResumenStock vresumenStock)
    {
        PopupBuscadorVisible = false;
        await Buscador.HideAsync();
        await OnRSSeleccionada.InvokeAsync(vresumenStock);
    }
}