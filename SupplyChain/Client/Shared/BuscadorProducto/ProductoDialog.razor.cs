using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Shared.BuscadorProducto;

public class ProductoDialogBase : ComponentBase
{
    protected List<Producto> productos = new();
    protected SfSpinner refSpinner;
    [Inject] public ProductoService ProductoService { get; set; }
    [Parameter] public bool PopupBuscadorVisible { get; set; }
    [Parameter] public EventCallback<Producto> OnObjectSelected { get; set; }
    [Parameter] public EventCallback OnCerrarDialog { get; set; }

    public async Task Show()
    {
        refSpinner?.ShowAsync();
        var response = await ProductoService.GetProdAndReparaciones();
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