using Microsoft.AspNetCore.Components;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.LinearGauge;
using Syncfusion.Blazor.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.EstadoPedidos
{
    public class EstadoPedidosBase : ComponentBase
    {
        [Inject] protected IRepositoryHttp Http { get; set; }

        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfToast ToastObj;
        protected SfGrid<vEstadoPedido> refSfGrid;
        protected List<vEstadoPedido> DataEstadosPedidos = new();
        protected vEstadoPedido PedidoSeleccionado = new();
        protected bool SpinnerVisible = false;

        protected bool VisibleDialog = false;
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Estados de Pedidos";
            SpinnerVisible = true;
            var response = await Http.GetFromJsonAsync<List<vEstadoPedido>>("api/EstadoPedidos");
            if (response.Error)
            {
                Console.WriteLine("ERROR OBTENER DATOS");
            }
            else
            {
                DataEstadosPedidos = response.Response;
                refSfGrid?.PreventRender();
                //await refSfGrid?.AutoFitColumnsAsync();
            }
            SpinnerVisible = false;
        }

        public void RowBound(RowDataBoundEventArgs<vEstadoPedido> Args)
        {
            if (Args.Data.ESTADO_PEDIDO != 9)
            {
                Args.Row.AddClass(new string[] { "e-removeEditcommand" });
            }
            else
            {
                Args.Row.AddClass(new string[] { "e-removeDeletecommand" });
            }
        }

        public void QueryCellInfoHandler(QueryCellInfoEventArgs<vEstadoPedido> args)
        {
            if (args.Data.ESTADO_PEDIDO == 1) /*"PEDIDO A CONFIRMAR"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-a-confirmar" });
            }
            else if (args.Data.ESTADO_PEDIDO == 2) /*"PEDIDO CONFIRMADO"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-confirmado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 3) /*"EN PRODUCCION"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-produccion" });
            }
            else if (args.Data.ESTADO_PEDIDO == 4) /*"CON TOTALIDAD DE COMPONENTES"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-componentes" });
            }
            else if (args.Data.ESTADO_PEDIDO == 5) /*"ARMADO Y CALIBRACION"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-armado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 6) /*"PENDIENTE DE REMITIR"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-pendiente-remitir" });
            }
            else if (args.Data.ESTADO_PEDIDO == 7) /*"A ENTREGAR"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-entregar" });
            }
            else if (args.Data.ESTADO_PEDIDO == 8) /*"ENTREGADO"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-entregado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 9) /*"FACTURADO"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-facturado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 10) /*ANULADO*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-anulado" });
            }
        }

        protected async Task OnRowSelected(RowSelectEventArgs<vEstadoPedido> arg)
        {
            arg.PreventRender = true;
        }

        protected async Task OnRowSelectedDouble_Click(RecordDoubleClickEventArgs<vEstadoPedido> arg)
        {
            arg.PreventRender = true;
            PedidoSeleccionado = arg.RowData;
            VisibleDialog = true;
        }

        protected void AxisLabelChange(AxisLabelRenderEventArgs args)
        {

            //1   PEDIDO A CONFIRMAR
            //2   PEDIDO CONFIRMADO
            //3   EN PROCESO
            //4   CON TOTALIDAD DE COMPONENTES
            //5   ARMADO Y CALIBRACION
            //6   PENDIENTE DE REMITIR
            //7   A ENTREGAR
            //8   ENTREGADO
            //9   FACTURADO
            //10  ANULADO

            if (args.Text == "1")
                args.Text = "PEDIDO A CONFIRMAR";
            else if (args.Text == "2")
                args.Text = "PEDIDO CONFIRMADO";
            else if (args.Text == "3")
                args.Text = "EN PROCESO";
            //else if (args.Text == "4")
            //    args.Text = "CON TOTALIDAD DE COMPONENTES";
            else if (args.Text == "4")
                args.Text = "ARMADO Y CALIBRACION";
            else if (args.Text == "6")
                args.Text = "PENDIENTE DE REMITIR";
            else if (args.Text == "7")
                args.Text = "A ENTREGAR";
            else if (args.Text == "8")
                args.Text = "ENTREGADO";
            //else if (args.Text == "9")
            //    args.Text = "FACTURADO";
            else
                args.Text = " ";
        }
    }
}
