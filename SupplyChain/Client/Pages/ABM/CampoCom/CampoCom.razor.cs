using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Pages.ABM.Prods;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.CampoCom
{
    public class CampoComBase : ComponentBase
    {
        [Inject] protected HttpClient Http {  get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        [Inject] protected CampoComService CampoComService { get; set; }

        #region "Vista Grilla"
        protected const string APPNAME = "grdCampoCom";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copia", TooltipText = "Copiar un comodin", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport"
        };

        protected List<CampoComodin> campos= new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<CampoComodin> refGrid;
        protected CampoComodin comodinSeleccionado = new ();
        protected bool popupFormVisible = false;
        protected bool SpinnerVisible = false;
        [CascadingParameter] MainLayout MainLayout { get; set; }

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "CampoCom";
            SpinnerVisible = true;
            var response = await CampoComService.Get();
            if (!response.Error)
            {
                campos = response.Response;
            }
            SpinnerVisible = false;
        }


        #region "Eventos Vista Grilla"
        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refGrid.SetPersistData(vistasGrillas.Layout);
        }
        protected async Task OnReinciarGrilla()
        {
            await refGrid.ResetPersistData();
        }
        #endregion

        protected async Task OnToolbarHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "Copy")
            {
                await Copiar();
            }
            else if (args.Item.Id == "grdCampoComDelete")
            {
                if((await refGrid.GetSelectedRecordsAsync()).Count > 0)
                {
                    bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que deseas eliminar el comodin?");
                    if (isConfirmed)
                    {
                        List<CampoComodin>campoABorrar = await refGrid.GetSelectedRecordsAsync();
                        var response= CampoComService.Eliminar(campoABorrar);
                        if(!response.IsCompletedSuccessfully)
                        {
                            await this.ToastObj.ShowAsync(new ToastModel
                            {
                                Title = "EXITO!",
                                Content = "los comodines seleccionados fueron eliminadas correctamente.",
                                CssClass = "e-toast-success",
                                Icon = "e-success toast-icons",
                                ShowCloseButton = true,
                                ShowProgressBar = true
                            });
                        }
                        else
                        {
                            await ToastMensajeError();
                        }
                    }
                }
            }
            else if (args.Item.Id == "grd_ExcelExport")
            {
                await refGrid.ExportToExcelAsync();
            }
            
        }

        protected async Task GuardarCampo(CampoComodin campo)
        {
            if (campo.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (campo.ESNUEVO)
                {
                    campos.Add(campo);
                }
                else
                {
                    var campoSinModificar = campos.Where(p => p.Id == campo.Id).FirstOrDefault();
                    campoSinModificar.Id = campo.Id;
                    campoSinModificar.Tabla = campo.Tabla;
                    campoSinModificar.Presion = campo.Presion;
                    campoSinModificar.Resorte= campo.Resorte;
                    campoSinModificar.Fluido = campo.Fluido;
                    campoSinModificar.Ajuste_Banco = campo.Ajuste_Banco;
                    campoSinModificar.Contra_Presion= campo.Contra_Presion;
                    campoSinModificar.Temperatura = campo.Temperatura;
                    campoSinModificar.CampoCom7= campo.CampoCom7;
                    campoSinModificar.CampoCom8= campo.CampoCom8;
                }
                await refGrid.RefreshHeaderAsync();
                refGrid.Refresh();
                await refGrid.RefreshColumnsAsync();
            }
            else
            {
                await ToastMensajeError();
            }
        }

        private async Task ToastMensajeExito()
        {
            await this.ToastObj.ShowAsync(new ToastModel
            {
                Title = "EXITO!",
                Content = "Guardado Correctamente.",
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        private async Task ToastMensajeError()
        {
            await ToastObj.ShowAsync(new ToastModel
            {
                Title = "Error!",
                Content = "Ocurrio un Error.",
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        private async Task Copiar()
        {
            if(refGrid.SelectedRecords.Count ==1)
            {
                comodinSeleccionado = new();
                CampoComodin selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que desea copiar el comodín?");
                if (isConfirmed)
                {   
                    comodinSeleccionado.ESNUEVO=true;
                    comodinSeleccionado.Tabla = selectedRecord.Tabla;
                    comodinSeleccionado.Presion = selectedRecord.Presion;
                    comodinSeleccionado.Resorte= selectedRecord.Resorte;
                    comodinSeleccionado.Fluido= selectedRecord.Fluido;
                    comodinSeleccionado.Ajuste_Banco= selectedRecord.Ajuste_Banco;
                    comodinSeleccionado.Contra_Presion= selectedRecord.Contra_Presion;
                    comodinSeleccionado.Temperatura= selectedRecord.Temperatura;
                    comodinSeleccionado.CampoCom7 = selectedRecord.CampoCom7;
                    comodinSeleccionado.CampoCom8 = selectedRecord.CampoCom8;

                    popupFormVisible=true;
                }
            }
            else
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Solo se puede copiar un item",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true

                });
            }
        }

        protected async Task OnActionBeginHandler(ActionEventArgs<CampoComodin> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
               args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                comodinSeleccionado = new();
                comodinSeleccionado.ESNUEVO= true;

            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                comodinSeleccionado = args.Data;
                comodinSeleccionado.ESNUEVO = false;
                //await refFormCampoCom.Refrescar(comodinSeleccionado);
            }

            if(args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                comodinSeleccionado= args.Data;
                comodinSeleccionado.ESNUEVO=false;
                //await refFormCampoCom.Refrescar(comodinSeleccionado);
            }
            
            if(args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
                 || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                refGrid.PreventRender();
                refGrid.Refresh();

                state= await refGrid.GetPersistDataAsync();
                await refGrid.AutoFitColumnsAsync();
                await refGrid.RefreshColumnsAsync();
                await refGrid.RefreshHeaderAsync();
            }
        }


        protected async Task OnActionCompleteHandler(ActionEventArgs<CampoComodin> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel= true;
                args.PreventRender = false;
                popupFormVisible=true;
            }
        }

        protected void OnCerrarDialog()
        {
            popupFormVisible = false;
        }
    }
}
