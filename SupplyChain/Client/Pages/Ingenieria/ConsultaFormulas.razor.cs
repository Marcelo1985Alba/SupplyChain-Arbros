using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ingenieria
{
    public class BaseConsultaFormulas : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfSpinner refSpinner;
        protected bool VisibleSpinner = false;

        protected SfDialog DialogDespieceRef;
        protected SfGrid<DespiecePlanificacion> GridDespiece;
        protected List<DespiecePlanificacion> listaDespiece = new();

        protected List<vIngenieriaProductosFormulas> DataOrdeProductosFormulas { get; set; } = new List<vIngenieriaProductosFormulas>();
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Consulta de Fórmulas";
            VisibleSpinner = true;
            DataOrdeProductosFormulas = await Http.GetFromJsonAsync<List<vIngenieriaProductosFormulas>>("api/Ingenieria/GetProductoFormulas");
            VisibleSpinner = false;
        }


        protected async Task CommandClickHandler(CommandClickEventArgs<vIngenieriaProductosFormulas> args)
        {

            if (args.CommandColumn.Title == "Despiece")
            {
                VisibleSpinner = true;
                listaDespiece = await Http.GetFromJsonAsync<List<DespiecePlanificacion>>("api/Planificacion/Despiece/" +
                    $"{args.RowData.CG_PROD.Trim()}/1/1");

                VisibleSpinner = false;
                await DialogDespieceRef.Show();
            }
        }

        protected async Task QueryCellInfoHandler(QueryCellInfoEventArgs<vIngenieriaProductosFormulas> args)
        {
            if (!args.Data.TIENE_FORM)
            {
                args.Cell.AddClass(new string[] { "rojas" });
            }
        }
    }
}
