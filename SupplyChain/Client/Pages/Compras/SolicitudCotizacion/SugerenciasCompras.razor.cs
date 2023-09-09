using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Inputs;

namespace SupplyChain.Client.Pages.Compras.SolicitudCotizacion;

public class SugerenciasComprasBase : ComponentBase
{
    protected List<Compra> DataSource = new();
    protected List<Compra> ListData = new();

    protected bool mostrarSpinnerSeleccionSugerencia;

    protected SfListBox<Compra[], Compra> refSfListBox;
    [Inject] protected IRepositoryHttp Http { get; set; }
    [Parameter] public EventCallback<Compra[]> OnItemsSeleccionados { get; set; }

    protected override async Task OnInitializedAsync()
    {
        mostrarSpinnerSeleccionSugerencia = true;
        await CargaSugerencia();
        mostrarSpinnerSeleccionSugerencia = false;
    }

    public async Task CargaSugerencia()
    {
        var response = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetSugerencia/");
        if (response.Error)
        {
            Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
            return;
        }

        ListData = DataSource = response.Response;
    }

    protected async Task SelectedChange(ListBoxChangeEventArgs<Compra[], Compra> args)
    {
        if (OnItemsSeleccionados.HasDelegate) await OnItemsSeleccionados.InvokeAsync(args.Value);
    }

    protected void OnInput(InputEventArgs eventArgs)
    {
        if (string.IsNullOrEmpty(eventArgs.Value))
            ListData = DataSource;
        else
            ListData = DataSource.FindAll(e => e.CG_MAT.ToLower().Contains(eventArgs.Value.ToLower())
                                               || e.DES_MAT.ToLower().Contains(eventArgs.Value.ToLower()));
    }

    public void ActualizarItems(List<Compra> sugerenciasActualizadas)
    {
        var sugerencias = ListData.Where(l => sugerenciasActualizadas.Contains(l)).ToList();
        foreach (var item in sugerencias) item.TieneSolicitudCotizacion = true;
    }
}