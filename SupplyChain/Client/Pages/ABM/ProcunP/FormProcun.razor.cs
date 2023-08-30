using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.ProcunP
{
    public class FormProcunBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public ProcunService ProcunService { get; set; }
        [Inject] public ProductoService ProductoService { get; set; }
        [Inject] public AreasService AreasService { get; set; }
        [Inject] public LineasService LineasService { get; set; }
        [Inject] public CeldasService CeldasService { get; set; }
        [Parameter] public Procun procuns { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Procun> OnGuardar { get; set; }
        [Parameter] public EventCallback<Procun> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }


        protected SfGrid<Procun> refGridItems;
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" },
            { "form","form-procun"}
        };
        protected bool IsAdd { get; set; }
        protected List<Producto> productos = new();
        protected List<Areas> areas = new();
        protected List<Lineas> lineas = new();
        protected List<Celdas> celdas = new();

        SfTreeView<Producto> tree;
        List<Producto> ListaDataProducto = new List<Producto>();
        List<Producto> Temp = new List<Producto>();
        public bool isFiltered = false;
        List<Producto> DataSource = new List<Producto>();
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
        protected async override Task OnInitializedAsync()
        {
            var response = await ProductoService.Get();
            if (!response.Error)
            {
                productos = response.Response;
            }
            var response2 = await AreasService.Get();
            if (!response2.Error)
            {
                areas = response2.Response;
            }
            var response3 = await LineasService.Get();
            if (!response3.Error)
            {
                lineas = response3.Response;
            }
            var response4 = await CeldasService.Get();
            if (!response4.Error)
            {
                celdas = response4.Response;
            }
            //ListaDataProducto = GetData();
        }

        //protected async List<Producto>GetData(){

        //    List<Producto> TempDataSource = new List<Producto>();
        //    return TempDataSource;
        //}

        //protected async void OnInput(InputEventArgs eventArgs)
        //{
        //    if (eventArgs.Value == "")
        //    {
        //        ListaDataProducto = GetData();
        //        isFiltered = false;
        //    }
        //    else
        //    {
        //        DataSource = GetData();
        //        List<Producto> filteredValues = DataSource.FindAll(e => e.Id.ToString().StartsWith(eventArgs.Value));
        //        List<Producto> filteredDataProducto = new List<Producto>();
        //        Producto data = new Producto();
        //        for (string i = 0; i < filteredValues.Count; i++)
        //        {
        //            data = filteredValues[i];
        //            if (data.Id == null)
        //            {
        //                data.Id= false;
        //                filteredDataProducto.Add(data);
        //                break;
        //            }
        //            while(data.Id != null)
        //            {
        //                if(fi)
        //            }
        //        }
        //    }
        //}

        protected async Task<bool> Agregar(Procun proc)
        {
            var response = await ProcunService.Existe(proc.Id);
            if (!response)
            {
                var response_2 = await ProcunService.Agregar(proc);
                if (response_2.Error)
                {
                    Console.WriteLine(await response_2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar la celda.");
                    return false;
                }
                procuns = response_2.Response;
                return true;
            }
            await ToastMensajeError($"El procun con registro {proc.Id} ya existe.\n\rO el procun no es permitido.");
            return false;
        }

        protected async Task<bool> Actualizar(Procun proc)
        {
            var response = await ProcunService.Actualizar(proc.Id, proc);
            if (response.Error)
            {
               await ToastMensajeError("Error al intentar Guardar el procun.");
               return false;
            }
            procuns = proc;
            return true;

        }

        protected async Task GuardarProc()
        {
            bool guardado;
            if (procuns.ESNUEVO)
            {
                guardado = await Agregar(procuns);
                procuns.ESNUEVO = true;
            }
            else
            {
                guardado = await Actualizar(procuns);
            }

            if (guardado)
            {
                Show = false;
                procuns.GUARDADO = guardado;
                await OnGuardar.InvokeAsync(procuns);
            }
        }


        public async Task Hide()
        {
            Show = false;
        }
        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
        }
        private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = content,
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        private async Task ToastMensajeError(string content = "Ocurrio un Error.")
        {
            await ToastObj.Show(new ToastModel
            {
                Title = "Error!",
                Content = content,
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }


    }
}
