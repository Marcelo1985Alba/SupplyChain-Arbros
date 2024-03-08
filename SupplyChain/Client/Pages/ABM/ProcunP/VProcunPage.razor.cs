using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.PivotView.Internal;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.ProcunP
{
    public class VProcunPageBase :ComponentBase
    {
        [Inject]protected HttpClient Http {  get; set; }
        [Inject]protected IJSRuntime JSRuntime { get; set; }
        [Inject]protected ProcunService ProcunService { get; set; }

        #region "Vista Grilla"
        protected const string APPNAME = "grdProcunABM";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        new ItemModel { Text = "Copia", TooltipText = "Copiar una celda", PrefixIcon = "e-copy", Id = "Copy" },
        new ItemModel{Text="Excel Export", Id="ExcelExport"}
        };
        protected FormProcun refFormProcun;
        protected List<vProcun> vprocuns = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<vProcun> refGrid;
        protected vProcun vProcunSeleccionado = new();
        protected bool SpinnerVisible=false;

        protected bool popupFormVisible = false;
        protected ConfirmacionDialog ConfirmacionDialog;

        [CascadingParameter] MainLayout MainLayout {  get; set; }

        #region "Eventos Vista Grilla"
        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refGrid.SetPersistDataAsync(vistasGrillas.Layout);
        }
        protected async Task OnReiniciarGrilla()
        {
            await refGrid.ResetPersistDataAsync();
        }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Procun";
            SpinnerVisible = true;
            await GetvProcun();
            SpinnerVisible=false;
        }



        protected async Task GetvProcun()
        {
            var response= await ProcunService.GetvProcun();
            if (response.Error)
            {
                await ToastMensajeError("Error al cargar los procesos.");
            }
            else
            {
                vprocuns = response.Response;
            }
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
        //protected async Task OnToobarHandler(ClickEventArgs args)
        //{
        //    if (args.Item.Id == "Copy")
        //    {
        //        //await CopiarvProcun();
        //    }
        //    else if(args.Item.Id == "grdvProcun_delete")
        //    {
        //        if((await refGrid.GetSelectedRecordsAsync()).Count > 0)
        //        {
        //            bool isConfirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Seguro que desea borrar el proceso?");
        //            if (isConfirmed)
        //            {
        //                List<Procun> vprocunABorrar= await refGrid.GetSelectedRecordsAsync();
        //                var response = ProcunService.Eliminar(vprocunABorrar);
        //                if
        //            }
        //        }
        //    }
        //}

    }
}
