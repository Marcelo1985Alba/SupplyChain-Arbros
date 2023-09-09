using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.Panel_Control.DetalleCategoria;

public class DialogDetalleBase : ComponentBase
{
    protected SfSpinner sfSpinner;

    protected bool visibliSpinner = false;

    [Parameter] public string Titulo { get; set; } = null!;
    [Parameter] public string Height { get; set; } = null!;
    [Parameter] public string Width { get; set; } = null!;
    [Parameter] public bool Visible { get; set; } = false;
    [Parameter] public bool MostrarVerMas { get; set; } = false;
    [Parameter] public bool IsModal { get; set; } = false;

    [Parameter] public RenderFragment Content { get; set; }

    [Parameter] public EventCallback<bool> OnCerrarDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        //visibliSpinner = true;
        //if (DataSource == null)
        //{
        //    visibliSpinner = true;
        //}
    }


    protected async Task OnAfterDialogOpned(BeforeOpenEventArgs arg)
    {
        visibliSpinner = true;
    }

    protected async Task OnAfterDialogClosed(object arg)
    {
        Visible = false;
        visibliSpinner = false;
        await OnCerrarDialog.InvokeAsync(Visible);
    }

    public async Task ShowAsync()
    {
        Visible = true;
        visibliSpinner = true;
        await InvokeAsync(StateHasChanged);
    }

    public async Task HideAsync()
    {
        Visible = false;
        visibliSpinner = false;
        await InvokeAsync(StateHasChanged);
    }
}