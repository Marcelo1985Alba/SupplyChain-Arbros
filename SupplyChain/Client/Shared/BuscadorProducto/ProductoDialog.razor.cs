using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared.BuscadorProducto
{
    public class ProductoDialogBase : ComponentBase
    {
        [Inject] public ProductoService ProductoService { get; set; }
        [Parameter] public bool PopupBuscadorVisible { get; set; } = false;
        [Parameter] public EventCallback<Producto> OnObjectSelected { get; set; }
        [Parameter] public EventCallback OnCerrarDialog { get; set; }
        [Parameter] public string Cg_Prod { get; set; } = "";
        [Parameter] public string Des_Producto { get; set; } = "";
        [Parameter] public bool conMP { get; set; } = true;
        [Parameter] public bool conSE { get; set; } = true;
        [Parameter] public bool conPT { get; set; } = true;


        protected List<Producto> productos = new();
        protected SfSpinner refSpinner;
        public async Task Show()
        {
         
            refSpinner?.ShowAsync();
            if (string.IsNullOrEmpty(Cg_Prod))
            {
                Cg_Prod = "VACIO";
            }

            if (string.IsNullOrEmpty(Des_Producto))
            {
                Des_Producto = "VACIO";
            }

            var response = await ProductoService.Search(Cg_Prod, Des_Producto);

            if (response.Error)
            {
                refSpinner?.HideAsync();
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                productos = response.Response;
                refSpinner?.HideAsync();
                PopupBuscadorVisible = true;
            }
            
            //await Search();
        }

        public async Task BuscarTodos()
        {
            var response = await ProductoService.Get();
            if(response.Error)
            {
                refSpinner?.HideAsync();
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                productos = response.Response;
                refSpinner?.HideAsync();
                PopupBuscadorVisible = true;
            }
        }

        protected async Task Search()
        {
            var response = await ProductoService.Search(Cg_Prod, Des_Producto);
            if (response.Error)
            {
                refSpinner?.HideAsync();
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                productos = response.Response;
                if (productos.Count == 1)
                {
                    await SendObjectSelected(productos[0]);
                }
                else
                {
                    PopupBuscadorVisible = true;
                }
                refSpinner?.HideAsync();
            }
        }


        public async Task Hide()
        {
            PopupBuscadorVisible = false;
        }
        protected async Task SendObjectSelected(Producto obj)
        {
            await OnObjectSelected.InvokeAsync(obj);
            await Hide();
        }

        protected async Task CerrarDialog()
        {
            await Hide();
            await OnCerrarDialog.InvokeAsync();
        }
    }
}
