using Microsoft.AspNetCore.Components;
using SupplyChain.Shared.PCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared
{
    public partial class BuscadorResumenStockBase : ComponentBase
    {
        [Inject] HttpClient Http { get; set; }
        [Parameter] public string TextoButon { get; set; } = "";
        [Parameter] public bool EsSoloLectura { get; set; } = false;
        [Parameter] public bool MostraBotonBusqueda { get; set; } = false;
        [Parameter] public bool HabilitaBotonBusqueda { get; set; } = false;
        [Parameter] public bool MostrarSpin { get; set; } = true;
        [Parameter] public EventCallback<vResumenStock> OnRSSeleccionada { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> HtmlAtrr { get; set; } = new Dictionary<string, object>() { { "type", "button" } };
        protected BuscadorEmergente<vResumenStock> Buscador;
        protected vResumenStock[] ItemsABuscar = null;
        protected string[] ColumnasBuscador = new string[] { "CG_ART", "CG_DEP", "DESPACHO", "SERIE", "LOTE" };
        protected string TituloBuscador = "Listado Insumos con Stock";
        protected bool PopupBuscadorVisible = false;

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
}
