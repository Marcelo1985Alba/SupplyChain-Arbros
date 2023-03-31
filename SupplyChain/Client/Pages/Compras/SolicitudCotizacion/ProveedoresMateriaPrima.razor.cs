using Microsoft.AspNetCore.Components;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Compras.SolicitudCotizacion
{
    public class ProveedoresMateriaPrimaBase: ComponentBase
    {
        [Inject] public IRepositoryHttp Http { get; set; }
        [Parameter] public bool SoloProveedoresConEmail { get; set; } = true;
        [Parameter] public int[] FiltroIdsProveedor { get; set; } = System.Array.Empty<int>();
        [Parameter] public EventCallback<vProveedorItris> OnProveedorSeleccionado { get; set; }
        [Parameter] public EventCallback<vProveedorItris> OnProveedorDeseleccionado { get; set; }

        protected bool mostrarSpinnerActualizando = false;
        public event Action<int[]> OnIdsProveedoresChanged;

        protected List<vProveedorItris> DataProveedores = new();
        protected List<vProveedorItris> DataProveedoresFiltrado = new();

        protected async override Task OnInitializedAsync()
        {
            await GetProveedoresMP();
        }

        protected async Task GetProveedoresMP()
        {
            string apiProveedores = "api/Proveedores/GetProveedoresItris";
            if (FiltroIdsProveedor is not null && FiltroIdsProveedor.Length > 0)
            {
                apiProveedores += $"?valores={JsonSerializer.Serialize(FiltroIdsProveedor)}";
            }

            var response = await Http.GetFromJsonAsync<List<vProveedorItris>>(apiProveedores);
            if (response.Error)
            {
                Console.WriteLine(response.HttpResponseMessage.ReasonPhrase); return;
            }
            else
            {
                DataProveedores = response.Response.ToList();

            }
        }

        protected void OnChange(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            FiltrarProveedores();
        }

        protected void FiltrarProveedores()
        {
            var data = DataProveedores.Where(d=> FiltroIdsProveedor.Contains(d.Id)).ToList();
            if (SoloProveedoresConEmail)
            {
                DataProveedoresFiltrado = data.Where(p => !string.IsNullOrEmpty(p.EMAIL_CONTACTO)).ToList();
            }
            else
            {
                DataProveedoresFiltrado = data.ToList();
            }
        }

        public void SetIntArray(int[] intArray)
        {
            mostrarSpinnerActualizando = true;
            FiltroIdsProveedor = intArray;
            FiltrarProveedores();
            OnIdsProveedoresChanged?.Invoke(FiltroIdsProveedor);
            mostrarSpinnerActualizando = false;
        }

        protected async Task RowSelected(RowSelectEventArgs<vProveedorItris> Args)
        {
            if (OnProveedorSeleccionado.HasDelegate)
            {
                await OnProveedorSeleccionado.InvokeAsync(Args.Data);
            }
        }

        protected async Task RowDeselected(RowDeselectEventArgs<vProveedorItris> Args)
        {
            if (OnProveedorDeseleccionado.HasDelegate)
            {
                await OnProveedorDeseleccionado.InvokeAsync(Args.Data);
            }
        }
    }
}
