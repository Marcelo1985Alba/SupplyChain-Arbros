using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.Procedimientos
{
    public class FormOperacionesBase :ComponentBase
    {
        [Inject] protected HttpClient http { get; set; }
        [Inject] public ProcedimientosService ProcedimientosService{ get; set; }
        [Inject] protected UnidadesService UnidadesService { get; set; }
        [Parameter] public Operaciones Operaciones { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Operaciones> OnGuardar { get; set; }
        [Parameter]public EventCallback OnCerrar { get; set; }

        protected SfGrid<Operaciones> refGridItems;
        protected List<Unidades> unidades = new();  
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVivible=false;
        protected SfToast ToastObj;
        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            {"type", "submit" }
        };
        protected bool IsAdd {  get; set; }

        protected override async Task OnInitializedAsync()
        {
           
            var response = await UnidadesService.Get();
             if (!response.Error)
             {
               unidades = response.Response;
             }    
        }



        protected async Task<bool> Agregar(Operaciones proc)
        {
            var response = await ProcedimientosService.Existe(proc.Id);
            if (!response)
            {
                var response2 = await ProcedimientosService.Agregar(proc);
                if (response2.Error)
                {
                    Console.WriteLine(await response2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar la operación.");
                    return false;
                }
                Operaciones = response2.Response;
                return true;
            }
            await ToastMensajeError($"La operación con codigo {proc.Id} ya existe.\n\rO el area no es permitida.");
            return false;
        }

        protected async Task<bool>Actualizar(Operaciones proc)
        {
            var response = await ProcedimientosService.Actualizar(proc.Id, proc);
            if (response.Error)
            {
                await ToastMensajeError("Error al intentar Guardar el procedimiento.");
                return false;
            }
            Operaciones = proc;
            return true;
        }

        protected async Task GuardarProc()
        {
            bool guardado=false;
            if (Operaciones.ESNUEVO)
            {
                guardado = await Agregar(Operaciones);

            }
            else
            {
                guardado= await Actualizar(Operaciones);
            }

            if (guardado)
            {
                Show = false;
                Operaciones.GUARDADO= guardado; 
                await OnGuardar.InvokeAsync(Operaciones);
            }
        }

        public async Task Refrescar(Operaciones operaciones)
        {
            Operaciones = operaciones;
            await InvokeAsync(StateHasChanged);
        }

        public async Task Hide()
        {
            Show=false;
        }

        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
        }


        private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
        {
            await this.ToastObj.ShowAsync(new ToastModel
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
            await ToastObj.ShowAsync(new ToastModel
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
