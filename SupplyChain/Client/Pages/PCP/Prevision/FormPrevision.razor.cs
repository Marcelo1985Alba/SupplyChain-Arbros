using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.PCP.Prevision
{
    public class FormPrevision : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public ProductoService ProductoService { get; set; }

        [Parameter]public Producto Producto { get; set; }
        [Parameter] public EventCallback<PresAnual> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected Producto prodSeleccionado = new();
        protected SfGrid<Producto> refGridItems;
        protected bool popupFormVisible = false;

        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refGrid.SetPersistData(vistasGrillas.Layout);
        }
        protected async Task OnReiniciarGrilla()
        {
            await refGrid.ResetPersistData();
        }

        protected async Task OnActionBeginHandler(ActionEventArgs<Producto> args)
        {
            if(args.RequestType==Action.Add||
                args.RequestType==Action.BeginEdit)
            {
                args.Cancel= true;
                args.PreventRender= false;
                popupFormVisible = true;

            }
        }
    }
}
