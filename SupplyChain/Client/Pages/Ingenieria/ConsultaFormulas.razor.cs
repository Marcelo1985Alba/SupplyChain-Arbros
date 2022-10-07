using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
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
        [Inject] public ExcelService ExcelService { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfTab refSfTab;
        protected SfSpinner refSpinner;
        protected bool VisibleSpinner = false;

        protected SfToast ToastObj;

        protected List<Object> Toolbaritems = new() { "ExcelExport",
            new ItemModel() { Type = ItemType.Separator },
            new ItemModel() { Text = "Guardar Excel", Type = ItemType.Button, CssClass = "", ShowTextOn = DisplayMode.Both,
                TooltipText = "Guardar el despiece en formato excel", PrefixIcon = "e-update e-icons", Id = "GuardarExcel" } };

        protected SfDialog DialogDespieceRef;
        protected SfGrid<DespiecePlanificacion> GridDespiece;
        protected SfGrid<vIngenieriaProductosFormulas> GridProdForm;
        protected List<DespiecePlanificacion> listaDespiece = new();
        protected List<vIngenieriaProductosFormulas> DataOrdeProductosFormulas { get; set; } = new List<vIngenieriaProductosFormulas>();
        protected vIngenieriaProductosFormulas ProdSelected = new();

        public class TabData
        {
            public string Header { get; set; }
            public string CodigoInsumo { get; set; }
            public RenderFragment Content { get; set; }
        }
        protected List<TabData> TabItems = new();

        protected string tituloTabFormulas = string.Empty;
        protected bool mostrarCerrarTab = false;
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
                ProdSelected = args.RowData;
                listaDespiece = await Http.GetFromJsonAsync<List<DespiecePlanificacion>>("api/Planificacion/Despiece/" +
                    $"{args.RowData.CG_PROD.Trim()}/1/1");

                VisibleSpinner = false;
                await DialogDespieceRef.Show();
            }
            else if (args.CommandColumn.Title == "Ver Plano")
            {
                VisibleSpinner = true;
                tituloTabFormulas = "Formulas";
                mostrarCerrarTab = true;
                TabItems.Add(new TabData
                {
                    Header = $"Plano ({args.RowData.CG_PROD.Trim()})",
                    //Verificar si es un semi o un prod, los semis obtener los primeros 7 digitos
                    CodigoInsumo = new string(args.RowData.CG_PROD.Take(2).ToArray()) == "C-" 
                                || new string(args.RowData.CG_PROD.Take(2).ToArray()) == "D-" ?
                    args.RowData.CG_PROD.Substring(0, 7) : args.RowData.CG_PROD

                });


                //El primer tab no se encuentra dentro de la lista
                //le resto el que agregue para que no lo desactive
                //for (int i = 0; i < TabItems.Count + 1; i++)
                //{
                //    await refSfTab.EnableTab(i, false);
                //}


                //await refSfTab.EnableTab(TabItems.Count, true);
                //await refSfTab.Select(TabItems.Count);
                VisibleSpinner = false;
            }
            else if (args.CommandColumn.Title == "Ver Programas")
            {
                VisibleSpinner = true;
                tituloTabFormulas = "Formulas";
                mostrarCerrarTab = true;
                TabItems.Add(new TabData
                {
                    Header = $"Programas ({args.RowData.CG_PROD.Trim()})",
                    CodigoInsumo = args.RowData.CG_PROD.Trim()

                });


                //El primer tab no se encuentra dentro de la lista
                //le resto el que agregue para que no lo desactive
                //for (int i = 0; i < TabItems.Count + 1; i++)
                //{
                //    await refSfTab.EnableTab(i, false);
                //}


                //await refSfTab.EnableTab(TabItems.Count, true);
                //await refSfTab.Select(TabItems.Count);
                VisibleSpinner = false;
            }

        }

        protected async Task ToolbarProdFormClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {

            if (args.Item.Id == "ProdForm_excelexport") //Id is combination of Grid's ID and itemname
            {
                VisibleSpinner = true;
                await this.GridProdForm.ExcelExport();
                VisibleSpinner = false;
            }
        }

        protected async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            
            if (args.Item.Id == "Despiece_excelexport") //Id is combination of Grid's ID and itemname
            {
                VisibleSpinner = true;
                await this.GridDespiece.ExportToExcelAsync();
                VisibleSpinner = false;
            }

            if (args.Item.Text == "Guardar Excel")
            {
                VisibleSpinner = true;
                var dataEspecial = listaDespiece.Select(d => new
                {
                    d.CG_PROD,
                    d.CG_SE,
                    d.CG_MAT,
                    d.DES_PROD,
                    d.CG_FORM,
                    CANT = d.CANT_MAT

                }).ToDataTable();
                await ExcelService.CreateExcel(dataEspecial, ProdSelected.CG_PROD.Trim());
                VisibleSpinner = false;

                await MostrarMensajeToastSuccess();

            }
        }

        private async Task MostrarMensajeToastSuccess()
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = "Guardado Correctamente.",
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }


        protected async Task QueryCellInfoHandler(QueryCellInfoEventArgs<vIngenieriaProductosFormulas> args)
        {
            if (!args.Data.TIENE_FORM)
            {
                args.Cell.AddClass(new string[] { "rojas" });
            }

            if (args.Data.TIENE_FORM && !args.Data.FORM_ACTIVA)
            {
                args.Cell.AddClass(new string[] { "amarillas" });
            }
        }

        protected async Task TabEliminando(RemoveEventArgs removeEventArgs)
        {
            var tab = refSfTab.Items[removeEventArgs.RemovedIndex];
            //if (string.IsNullOrEmpty(tab.Header.Text) || tab.Header.Text == "Formulas")
            //{
            //    removeEventArgs.Cancel = true;
            //}
            if (removeEventArgs.RemovedIndex == 0)
            {
                removeEventArgs.Cancel = true;
            }
            //if (refSfTab.Items.Count == 1)
            //{
            //    mostrarCerrarTab = false;
            //}

            
        }


        protected async Task TabEliminado(RemoveEventArgs removeEventArgs)
        {
            if (refSfTab.Items.Count == 1)
            {
                mostrarCerrarTab = false;
                tituloTabFormulas = string.Empty;
            }
        }
    }
}
