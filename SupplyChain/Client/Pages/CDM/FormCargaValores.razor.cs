//using Microsoft.AspNetCore.Components;
//using SupplyChain.Client.HelperService;
//using Syncfusion.Blazor.Grids;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Syncfusion.Blazor.Spinner;
//using Syncfusion.Blazor.Notifications;
//using SupplyChain.Shared;

//namespace SupplyChain.Client.Pages.CDM
//{
//    public class FormCargaValoresBase : ComponentBase
//    {
//        [Inject] protected HttpClient Http { get; set; }
//        //[Inject] public CargaValoresService CargaValoresService { get; set; }
//        [Inject] public InventarioService InventarioService { get; set; }
//        [Parameter] public Pedidos valor { get; set; } = new();
//        [Parameter] public bool Show { get; set; } = false;
//        [Parameter] public EventCallback<Pedidos> OnGuardar { get; set; }
//        [Parameter] public EventCallback<Pedidos> OnEliminar { get; set; }
//        [Parameter] public EventCallback OnCerrar { get; set; }

//        protected SfGrid<CargaValoresDetalles> refGridItems;
//        protected SfSpinner refSpinnerCli;
//        protected bool SpinnerVisible = false;
//        protected SfToast ToastObj;
//        protected Dictionary<string, object> HtmlAttributeSubmint = new()
//        {
//            {"type", "submit" }
//        };
//        protected bool IsAdd { get; set; }

//        protected async Task<bool> Agregar(Valores valor)
//        {
//            var response = await InventarioService.Existe(valor.Id);
//            if (!response)
//            {
//                var response2 = await InventarioService.Agregar(valor);
//                if (response2.Error)
//                {
//                    Console.WriteLine(await response2.HttpResponseMessage.Content.ReadAsStringAsync());
//                    await ToastMensajeError("Error al intentar Guardar un Proceso.");
//                    return false;
//                }
//                valor = response2.Response;
//                return true;
//            }
//            await ToastMensajeError($"El proceso con codigo{valor.Id} ya existe.\n\rO el proceso no es permitido.");
//            return false;
//        }

//        protected async Task<bool> Actualizar(Pedidos valor)
//        {
//            var response = await InventarioService.Actualizar(valor.Id, valor);
//            if (response.Error)
//            {
//                await ToastMensajeError("Error al intenar guardar el Proceso.");
//                return false;
//            }
//            return true;
//        }

//        protected async Task GuardarProceso()
//        {
//            bool guardado = false;
//            if (valor.ESNUEVO)
//            {
//                guardado = await Agregar(valor);
//            }
//            else
//            {
//                guardado = await Actualizar(valor);
//            }
//            if (guardado)
//            {
//                Show = false;
//                valor.GUARDADO = guardado;
//                await OnGuardar.InvokeAsync(valor);
//            }
//        }

//        /* protected async Task GetItem(int id)
//         {
//             var response = await CargaValoresService.GetById(id);
//             if (response.Error)
//             {
//                 await ToastMensajeError("Error ******");
//             }
//             else
//             {
//                 valor = response.Response;
//                 foreach(var item in valor)
//                 {
//                     item.Estado = SupplyChain.Shared.Enum.EstadoItem.Modificado;
//                 }
//             }
//         }*/

//        protected async Task BatchDeleteHandler(BeforeBatchDeleteArgs<CargaValoresDetalles> args)
//        {
//            var item = valor.Items.Find(i => i.Id == args.RowData.Id);
//            item.Estado = SupplyChain.Shared.Enum.EstadoItem.Eliminado;
//        }
//        public async Task CellSavedHandler(CellSaveArgs<CargaValoresDetalles> args)
//        {
//            var index = await refGridItems.GetRowIndexByPrimaryKey(args.RowData.Id);
//            if (args.ColumnName== "SINMON")
//            {
                
//            }
//        }
//        public async Task Hide()
//        {
//            Show = false;
//        }
//        protected async Task OnCerrarDialog()
//        {
//            await OnCerrar.InvokeAsync();
//        }
//        private async Task ToastMensajeExito(string content="Guardado Correctamente.")
//        {
//            await this.ToastObj.Show(new ToastModel
//            {
//                Title = "EXITO!",
//                Content = content,
//                CssClass = "e-toast-success",
//                Icon = "e-warning toast-icons",
//                ShowCloseButton = true,
//                ShowProgressBar = true
//            });
//        }
//        private async Task ToastMensajeError(string content = "Ocurrio un error.")
//        {
//            await ToastObj.Show(new ToastModel
//            {
//                Title = "EXITO!",
//                Content = content,
//                CssClass = "e-toast-success",
//                Icon = "e-warning toast-icons",
//                ShowCloseButton = true,
//                ShowProgressBar = true
//            });

//        }
//    }
//}
