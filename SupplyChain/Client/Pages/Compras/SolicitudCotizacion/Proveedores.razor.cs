using Microsoft.AspNetCore.Components;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Compras.SolicitudCotizacion
{
    public class ProveedoresBase:ComponentBase
    {
        [Inject] public IRepositoryHttp Http { get; set; }
        [Parameter] public int[] IdsProveedoresConMp { get; set; } = Array.Empty<int>();
        [Parameter] public bool SoloProveedoresConEmail { get; set; } = true;

        public event Action<int[]> OnIdsProveedoresChanged;

        protected bool mostrarSpinnerCargando = false;
        protected List<vProveedorItris> DataProveedores = new();
        protected List<vProveedorItris> DataProveedoresFiltrado = new();
        protected async override Task OnInitializedAsync()
        {
            mostrarSpinnerCargando = true;
            var response = await Http.GetFromJsonAsync<List<vProveedorItris>>("api/Proveedores/GetProveedoresItris");
            if (response.Error)
            {

            }
            else
            {
                DataProveedores = response.Response.OrderBy(o=> o.Id).ToList();
                FiltrarProveedores();
            }
            mostrarSpinnerCargando = false;
        }
        protected void OnChange(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            mostrarSpinnerCargando = true;
            FiltrarProveedores();
            mostrarSpinnerCargando = false;
        }

        protected void FiltrarProveedores()
        {
            var data = DataProveedores.Where(d => !IdsProveedoresConMp.Contains(d.Id)).ToList();
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
            mostrarSpinnerCargando = true;
            IdsProveedoresConMp = intArray;
            FiltrarProveedores();
            OnIdsProveedoresChanged?.Invoke(IdsProveedoresConMp);
            mostrarSpinnerCargando = false;
        }
    }
}
