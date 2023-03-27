using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.LinearGauge;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace SupplyChain.Client.Pages.EstadoCompras
{
    public class EstadoComprasBase : ComponentBase
    {
        [Inject] protected IRepositoryHttp Http { get; set; }
        [Inject] protected PdfService PdfService{ get; set; }
        [Inject] protected IJSRuntime JS { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }

        protected SfToast ToastObj;
        protected SfGrid<vESTADOS_COMPRAS> refSfGrid;
        protected List<vESTADOS_COMPRAS> DataEstadosCompras = new();
        protected vESTADOS_COMPRAS ComprasSeleccionado = new();
        protected bool SpinnerVisible = false;

        protected bool VisibleDialog = false;
        protected List<Object> Toolbaritems = new List<Object>()
        {
           "Search",
             new ItemModel {Text = "Excel Export", TooltipText="Excel Export", PrefixIcon="e-excelexport", Id="Excel Export"},
         };

        protected const string APPNAME = "grdEstadoCompras";
        protected string state;
        public async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refSfGrid.SetPersistData(vistasGrillas.Layout);
        }   
        public async Task OnReiniciarGrilla()
        {
            await refSfGrid.ResetPersistData();
        }


        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Estado de Compras";
            SpinnerVisible = true;
            var response = await Http.GetFromJsonAsync<List<vESTADOS_COMPRAS>>
                ("api/EstadoCompras");
            if (response.Error)
            {
                Console.WriteLine("ERROR!!");
                Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
            }
            else
            {
                //RESPONSE TIENE QUE VENIR CON DATOS DE LA VISTA
                //LA VARIABLE DATAESTADOCOMPRAS LA LLAMA EL DATASOURCE
                DataEstadosCompras = response.Response;
                refSfGrid?.PreventRender();
            }
            SpinnerVisible = false;
        }
        public async Task BeginHandler(ActionEventArgs<vESTADOS_COMPRAS> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                //VisibleProperty = true;
                refSfGrid.PreventRender();
                refSfGrid.Refresh();

                state = await refSfGrid.GetPersistData();
                await refSfGrid.AutoFitColumnsAsync();
                await refSfGrid.RefreshColumns();
                await refSfGrid.RefreshHeader();
                //VisibleProperty = false;
            }
        }

        protected async Task OnRowSelected(RowSelectEventArgs<vESTADOS_COMPRAS> arg)
        {
            arg.PreventRender=true;
        }
        protected async Task OnRowSelectedDouble_Click(RecordDoubleClickEventArgs<vESTADOS_COMPRAS> arg)
        {
            arg.PreventRender= true;
            ComprasSeleccionado = arg.RowData;
            VisibleDialog= true;
        }
        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if(args.Item.Text=="Seleccionar Columnas")
            {
                await refSfGrid.OpenColumnChooser(200, 50);
            }
            if(args.Item.Text=="Exportar grilla en Excel")
            {
                await this.refSfGrid.ExcelExport();
            }
        }
        //  1.Solicitar cotizacion
        //2.Pendiente de generar compra
        //3.A la esperad de cotizacion
        //4.Pendiente de entrega
        //5.Pagada
        //6.Vencida
        //7.Cerrada

        public void QueryCellInfoHandler(QueryCellInfoEventArgs<vESTADOS_COMPRAS> args)
        {
            if (args.Data.ESTADOS_COMPRA == "Solicitar Cotizacion") //SOLICITAR COTIZACION
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-solicitar-cotizacion" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Pendiente de generar compra") //PENDIENTE A COTIZACION
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-pendiente-generar-compra" });
            }
            else if(args.Data.ESTADOS_COMPRA=="A la espera de Cotizacion")
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-a-espera-cotizacion" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Pendiente de entrega")
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-pendiente-entrega" });
            }
            //else if (args.Data.ESTADOS_COMPRA == "Entrega Parcial")
            //{
            //    args.Cell.AddClass(new string[] { "color-estado-entrega-parcial" });
            //}
            else if (args.Data.ESTADOS_COMPRA == "Pagada") //PENDIENTE DE ENTREGA
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-pagada" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Vencida") //VENCIDA
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-vencida" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Cerrada")//cerrada
            {
                args.Cell.AddClass(new string[] {"color-estado-compra-cerrada" });
            }
            
            //else if (args.Data.ESTADOS_COMPRA == 7) //PENDIENTE DE PAGO
            //{
            //    args.Cell.AddClass(new string[] { "color-estado-compra-pendiente-pago" });
            //}
            //else if (args.Data.ESTADOS_COMPRA == 8) //PAGADA
            //{
            //    args.Cell.AddClass(new string[] { "color-estado-compra-pagada" });
            //}
            //else if (args.Data.ESTADOS_COMPRA == 9)//ANULADA
            //{
            //    args.Cell.AddClass(new string[] { "color-estado-compra-anulada" });
            //}
           

        }

        protected void AxisLabelChange(AxisLabelRenderEventArgs args)

        {
            //  1.Solicitar cotizacion
            //2.Pendiente de cotizacion
            //3.Pendiente de autorizar compra
            //4.Pendiente de entrega
            //5.Vencida
            //6.Cerrada
            //7.Pendiente de Pago
            //8.Pagada(tiene recibo
            //9.Anulada

            if (args.Text == "1")
                args.Text = "Solicitar Cotizacion";
            else if (args.Text == "2")
                args.Text = "Pendiente de generar compra";
            else if (args.Text == "3")
                args.Text = "A la espera de Cotizacion";
            else if (args.Text == "4")
                args.Text = "Pendiente de entrega";
            //else if (args.Text == "5")
            //    args.Text = "Entrega Parcial";
            else if (args.Text == "5")
                args.Text = "Pagada";
            else if (args.Text == "6")
                args.Text = "Vencida";
            else if (args.Text == "7")
                args.Text = "Cerrada";
            //else if (args.Text == "9")
            //    args.Text = "ANULADA";
            else
                args.Text = " ";    

        }
        public async Task CommandClickHandler(CommandClickEventArgs<vESTADOS_COMPRAS> args)
        {
            if (args.CommandColumn.ID == "btnDescargarRemito")
            {
                await DescargarRemito(args.RowData);
            }
            if (args.CommandColumn.ID == "btnDescargarFactura")
            {
                await DescargarFactura(args.RowData);
            }
        }

        private async Task DescargarRemito(vESTADOS_COMPRAS compras)
        {
            var responseFactura= await Http
                                .GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivosCompras/ByParamRuta/RUTAREMITO/{compras.REMITO}");
            if (responseFactura.Error)
            {
                Console.WriteLine("ERROR AL OBTENER REMITO");
                Console.WriteLine(await responseFactura.HttpResponseMessage.Content.ReadAsStringAsync());

            }
            else
            {
                if (responseFactura.Response.Count > 0)
                {
                    await JS.SaveAs(responseFactura.Response[0].Nombre, responseFactura.Response[0].ContenidoByte);
                }
                else
                {

                }
            }
        }

        private async Task DescargarFactura (vESTADOS_COMPRAS compras)
        {
            var formatoFacturaBuscar = compras.LETRA_FACTURA + compras.FACTURA;
            var responseFactura = await Http
                                .GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivosCompras/ByParamRuta/RUTAFACTURA/{formatoFacturaBuscar}");
            if (responseFactura.Error)
            {
                Console.WriteLine("ERROR AL OBTENER LA FACTURA");
                Console.WriteLine(await responseFactura.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                if(responseFactura.Response.Count> 0)
                {
                    await JS.SaveAs(responseFactura.Response[0].Nombre, responseFactura.Response[0].ContenidoByte);

                }
                else
                {

                }
            }
        }

        public void RowBound(RowDataBoundEventArgs<vESTADOS_COMPRAS> Args)
        {
            if (string.IsNullOrEmpty(Args.Data.REMITO))
            {
                Args.Row.AddClass(new string[] { "e-removedEditCommand" });
            }
            if (string.IsNullOrEmpty(Args.Data.FACTURA))
            {
                Args.Row.AddClass(new string[] { "e-removedDeleteCommand" });
            }
        }

        protected async Task OnToolBarHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "Excel Export")
            {
                await refSfGrid.ExportToExcelAsync();
            }
        }
    }
}
