using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared
{
    public partial class BuscadorEmergenteBase<TItem>:ComponentBase
    {
        
        [Parameter] public string Titulo { get; set; } = null!;
        [Parameter] public string Height { get; set; } = null!;
        [Parameter] public string Width { get; set; } = null!;
        [Parameter] public bool Visible { get; set; } = false;
        [Parameter] public bool MostrarVerMas { get; set; } = false;
        [Parameter] public IEnumerable<TItem> DataSource { get; set; }
        [Parameter] public string[] Columnas { get; set; } = null!;
        [Parameter] public EventCallback<TItem> OnObjetoSeleccionado { get; set; }
        [Parameter] public EventCallback OnBuscarMas { get; set; }
        [Parameter] public EventCallback<bool> OnCerrarDialog { get; set; }
        
        protected SfSpinner sfSpinner;
        protected SfGrid<TItem> Grid;
        protected bool visibliSpinner = false;
        protected List<Object> Toolbaritems = new(){
            "Search",
            new ItemModel(){ Type = ItemType.Separator},
            new ItemModel(){ Type = ItemType.Separator},
            "ExcelExport"
        };
        public TItem Selected { get; set; }

        protected async override Task OnInitializedAsync()
        {
            //visibliSpinner = true;
            //if (DataSource == null)
            //{
            //    visibliSpinner = true;
            //}
        }

        protected async Task OnLoadGrid(object args)
        {
            await sfSpinner.ShowAsync();
            visibliSpinner = true;
            await Grid.AutoFitColumns();
        }
        protected async Task OnDataBoundGrid(BeforeDataBoundArgs<TItem> args)
        {
            visibliSpinner = true;
        }
        protected async Task DataBoundGrid()
        {
            visibliSpinner = false;
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "ExcelExport")
            {
                await Grid.ExportToExcelAsync();
            }
        }
        public void EnviarObjetoSeleccionado()
        {
            Visible = false;
            OnObjetoSeleccionado.InvokeAsync(Selected);
        }


        protected void GetSelectedRecords(RowSelectEventArgs<TItem> args)
        {
            args.PreventRender = true; //without this, you may see noticable delay in selection with 75 rows in grid.
                                       //var items = await this.Grid.GetSelectedRecords();
            Selected = args.Data;
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

        protected async Task Buscar()
        {
            visibliSpinner = true;
            await OnBuscarMas.InvokeAsync();
            visibliSpinner = false;
        }
    }
}
