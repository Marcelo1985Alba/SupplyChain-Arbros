using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.MantCeldasP
{
    public class FormMantCeldasBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public MantCeldasService MantCeldasService { get; set; }
        [Inject] protected CeldasService CeldasService { get; set; }
        [Parameter] public MantCeldas mantCeldas { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<MantCeldas> OnGuardar { get; set; }
        [Parameter] public EventCallback<MantCeldas> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<MantCeldas> refGridItems;
        protected List<Celdas> celda = new();
        protected List<Operario> operario = new();
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" }
        };
        protected class MantenimientoOptions
        {
            public string Text { get; set; }
        }
        protected List<MantenimientoOptions> MantenimientoData = new List<MantenimientoOptions> {
            new MantenimientoOptions() {Text= "Preventivo" },
            new MantenimientoOptions() {Text= "Correctivo" },
        };
        protected class EstadoOptions
        {
            public string Text { get; set; }
        }
        protected List<EstadoOptions> EstadoData = new List<EstadoOptions> {
            new EstadoOptions() {Text= "Programado" },
            new EstadoOptions() {Text= "Realizado" },
            new EstadoOptions() {Text= "Cancelado" },
        };
        protected bool IsAdd { get; set; }
        protected async override Task OnInitializedAsync()
        {
            var response = await CeldasService.Get();
            if (!response.Error)
            {
                celda = response.Response;
            }
            operario = await Http.GetFromJsonAsync<List<Operario>>("api/Operario");
        }
        protected async Task<bool> Agregar(MantCeldas mantCelda)
        {
            mantCelda.Des_Celda = celda.Where(a => a.Id == mantCelda.Cg_Celda).FirstOrDefault().DES_CELDA;
            var response = await MantCeldasService.Existe(mantCelda.Id);
            if (!response)
            {
                var response_2 = await MantCeldasService.Agregar(mantCelda);
                if (response_2.Error)
                {
                    Console.WriteLine(await response_2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar la mantCelda.");
                    return false;
                }
                mantCeldas = response_2.Response;
                return true;
            }
            await ToastMensajeError($"La mantCelda con codigo {mantCelda.Id} ya existe.\n\rO la mantCelda no es permitida.");
            return false;
        }

        protected async Task<bool> Actualizar(MantCeldas mantCelda)
        {
            mantCelda.Des_Celda = celda.Where(a => a.Id == mantCelda.Cg_Celda).FirstOrDefault().DES_CELDA;
            var response = await MantCeldasService.Actualizar(mantCelda.Id, mantCelda);
            if (response.Error)
            {
                await ToastMensajeError("Error al intentar Guardar la mantCelda.");
                return false;
            }
            mantCeldas = mantCelda;
            return true;
        }

        protected async Task GuardarMantCelda()
        {
            bool guardado = false;
            if (mantCeldas.ESNUEVO)
            {
                guardado = await Agregar(mantCeldas);
            }
            else
            {
                guardado = await Actualizar(mantCeldas);
            }
            if (guardado)
            {
                Show = false;
                mantCeldas.GUARDADO = guardado;
                await OnGuardar.InvokeAsync(mantCeldas);
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
