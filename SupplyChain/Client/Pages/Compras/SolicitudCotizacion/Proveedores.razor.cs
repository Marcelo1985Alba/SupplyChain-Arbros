using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;

namespace SupplyChain.Client.Pages.Compras.SolicitudCotizacion;

public class ProveedoresBase : ComponentBase
{
    protected List<vProveedorItris> DataProveedores = new();
    protected List<vProveedorItris> DataProveedoresFiltrado = new();
    protected bool mostrarSpinnerCargando;

    protected SfGrid<vProveedorItris> refGrid;
    [Inject] public IRepositoryHttp Http { get; set; }
    [Parameter] public int[] IdsProveedoresConMp { get; set; } = Array.Empty<int>();
    [Parameter] public bool SoloProveedoresConEmail { get; set; } = true;
    [Parameter] public EventCallback<vProveedorItris> OnProveedorSeleccionado { get; set; }
    [Parameter] public EventCallback<vProveedorItris> OnProveedorDeseleccionado { get; set; }

    /// <summary>
    ///     Evento que se dispara cuando cambian los proveedores de las sugerencias
    /// </summary>
    public event Action<int[]> OnIdsProveedoresChanged;

    protected override async Task OnInitializedAsync()
    {
        mostrarSpinnerCargando = true;
        var response = await Http.GetFromJsonAsync<List<vProveedorItris>>("api/Proveedores/GetProveedoresItris");
        if (response.Error)
        {
        }
        else
        {
            DataProveedores = response.Response.OrderBy(o => o.Id).ToList();
            FiltrarProveedores();
        }

        mostrarSpinnerCargando = false;
    }

    protected void OnChange(ChangeEventArgs args)
    {
        mostrarSpinnerCargando = true;
        FiltrarProveedores();
        mostrarSpinnerCargando = false;
    }

    protected void FiltrarProveedores()
    {
        var data = DataProveedores.Where(d => !IdsProveedoresConMp.Contains(d.Id)).ToList();
        if (SoloProveedoresConEmail)
            DataProveedoresFiltrado = data.Where(p => !string.IsNullOrEmpty(p.EMAIL_CONTACTO)).ToList();
        else
            DataProveedoresFiltrado = data.ToList();
    }


    /// <summary>
    ///     Aplicar cambios de proveedores a mostrar
    /// </summary>
    /// <param name="intArray"></param>
    public void SetIntArray(int[] intArray)
    {
        mostrarSpinnerCargando = true;
        IdsProveedoresConMp = intArray;
        FiltrarProveedores();
        OnIdsProveedoresChanged?.Invoke(IdsProveedoresConMp);
        mostrarSpinnerCargando = false;
    }

    protected async Task RowSelected(RowSelectEventArgs<vProveedorItris> Args)
    {
        if (OnProveedorSeleccionado.HasDelegate) await OnProveedorSeleccionado.InvokeAsync(Args.Data);
    }

    protected async Task RowDeselected(RowDeselectEventArgs<vProveedorItris> Args)
    {
        if (OnProveedorDeseleccionado.HasDelegate) await OnProveedorDeseleccionado.InvokeAsync(Args.Data);
    }
}