﻿using Microsoft.AspNetCore.Components;
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
using System.Runtime.CompilerServices;
using SupplyChain.Shared.Enum;
using Syncfusion.Blazor.Data;


namespace SupplyChain.Client.Pages.EstadoCompras
{
    public class EstadoComprasBase : ComponentBase
    {
        [Inject] protected IRepositoryHttp Http { get; set; }
        [Inject] protected PdfService PdfService { get; set; }
        [Inject] protected IJSRuntime JS { get; set; }
        [Inject] protected EstadoComprasService EstadoComprasService { get; set; }
       // [Inject] protected EstadoComprasController EstadoComprasController { get; set; }
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
                new ItemModel{Text="Ver Todos", Id="Todos"},
             new ItemModel{Text="Ver Pendiente", Id="Pendiente"}
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
            await GetCompras(SupplyChain.Shared.Enum.EstadoCompras.TodosPendientes);
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
            arg.PreventRender = true;
        }
        protected async Task OnRowSelectedDouble_Click(RecordDoubleClickEventArgs<vESTADOS_COMPRAS> arg)
        {
            arg.PreventRender = true;
            ComprasSeleccionado = arg.RowData;
            VisibleDialog = true;
        }
        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Seleccionar Columnas")
            {
                await refSfGrid.OpenColumnChooserAsync(200, 50);
            }
            if (args.Item.Text == "Exportar grilla en Excel")
            {
                await this.refSfGrid.ExportToExcelAsync();
            }
        }
        //  1.Solicitar cotizacion
        //2.Pendiente de generar compra
        //3.A la esperad de cotizacion
        //4.Pendiente de entrega
        //5.Pagada
        //6.Vencida
        //7.Cerrada

        //public async Task QueryCellInfoHandler2(QueryCellInfoEventArgs<vESTADOS_COMPRAS> args)
        //{
        //    //DateTime fe_actual= DateTime.Now;
        //    //int diasPasados = 5;
        //    //DateTime fechaLimite = args.Data.FE_VENC.AddDays(diasPasados);
        //    //if(args.Data.FE_VENC.HasValue)
        //    //{

        //    //}

        //    if(args.Data.ESTADOS_COMPRA== "Pendiente Em.Solicitud de Cotizacion" && args.Data.ESTADOS_COMPRA=="Pendiente Entrega Vencida" && args.Data.FE_VENC.HasValue)
        //    {
        //        DateTime fechaActual = DateTime.Now;
        //        int diasPasados = 5;
        //        DateTime fechaLimite = args.Data.FE_VENC.Value.AddDays(diasPasados);
        //        if (fechaActual > fechaLimite)
        //        {
        //            args.Cell.AddClass(new string[] { "color-fecha-vencimiento-pasados" });
        //        }

        //    }

        //}

       

        public void QueryCellInfoHandler(QueryCellInfoEventArgs<vESTADOS_COMPRAS> args)
        {
            if (args.Data.ESTADOS_COMPRA == "Pendiente Em.Solicitud de Cotizacion") //SOLICITAR COTIZACION
            {
                DateTime fechaActual = DateTime.Now;
                int diasPasados = 5;
                DateTime fechaLimite = args.Data.FE_VENC.Value.AddDays(diasPasados);
                args.Cell.AddClass(new string[] { "color-estado-compra-pendiente-emitir-solcot", "color-fecha-vencimiento-pasados" } 
                );
                
            }
            else if (args.Data.ESTADOS_COMPRA == "Pendiente Emision OC") //PENDIENTE A COTIZACION
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-pendiente-emision-oc" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Pendiente Entrega en Fecha")
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-pendiente-entrega-fecha" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Pendiente Entrega Vencida")
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-pendiente-entrega-vencida" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Recibida Parcial-Pendiente de Pago") //PENDIENTE DE ENTREGA
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-recibida-parcial-pendiente-pago" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Recibida Total-Pendiente de Pago") //VENCIDA
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-recibida-total-pendiente-pago" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Pagada-Recibida")//cerrada
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-pagada-recibida" });
            }
            else if (args.Data.ESTADOS_COMPRA == "Cerrada") //PENDIENTE DE PAGO
            {
                args.Cell.AddClass(new string[] { "color-estado-compra-cerrada" });
            }
            //if (args.Data.FE_VENC.HasValue)
            //{
            //    DateTime fechaActual = DateTime.Now;
            //    int diasPasados = 5;
            //    DateTime fechaLimite = args.Data.FE_VENC.Value.AddDays(diasPasados);

            //    if (fechaActual > fechaLimite)
            //    {
            //        args.Cell.AddClass(new string[] { "color-fecha-vencimiento-pasados" });
            //    }
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
                args.Text = "Pendiente Em.Solicitud de Cotizacion";
            else if (args.Text == "2")
                args.Text = "Pendiente Emision OC";
            else if (args.Text == "3")
                args.Text = "Pendiente Entrega en Fecha";
            else if (args.Text == "4")
                args.Text = "Pendiente Entrega Vencida";
            else if (args.Text == "5")
                args.Text = "Recibida Parcial-Pendiente de Pago";
            else if (args.Text == "6")
                args.Text = "Recibida Total-Pendiente de Pago";
            else if (args.Text == "7")
                args.Text = "Pagada-Recibida";
            else if (args.Text == "8")
                args.Text = "Cerrada";
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
            var responseFactura = await Http
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

        private async Task DescargarFactura(vESTADOS_COMPRAS compras)
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
                if (responseFactura.Response.Count > 0)
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

        protected async Task GetCompras(SupplyChain.Shared.Enum.EstadoCompras estadoCompras = SupplyChain.Shared.Enum.EstadoCompras.Todos)
        {
            var response = await EstadoComprasService.ByEstado(estadoCompras);
            if (response.Error)
            {

            }
            else
            {
                DataEstadosCompras = response.Response.OrderByDescending(P => P.ESTADOS_COMPRA).ToList();
            }
        }

        protected async Task OnToolBarHandler(ClickEventArgs args)
        {
            {
                if (args.Item.Id == "Excel Export")
                {
                    await refSfGrid.ExportToExcelAsync();
                }
                else if (args.Item.Id == "Pendiente")
                {
                    SpinnerVisible = true;
                    await GetCompras(SupplyChain.Shared.Enum.EstadoCompras.TodosPendientes);

                    SpinnerVisible = false;
                }
                else if (args.Item.Id == "Todos")
                {
                    SpinnerVisible = true;
                    await GetCompras();
                    SpinnerVisible = false;
                }

            }
        }
    }
}
