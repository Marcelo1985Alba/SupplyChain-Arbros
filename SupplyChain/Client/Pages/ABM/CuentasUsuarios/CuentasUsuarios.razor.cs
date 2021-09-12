using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Grids;
using SupplyChain.Shared.Login;

namespace SupplyChain.Client.Pages.ABM.CuentasUsuarios
{
    public class CuentasUsuariosBase: ComponentBase
    {
        [Inject] public IRepositorio Http { get; set; }
        protected SfToast ToastObj;
        protected SfSpinner SpinnerObj;
        protected SfGrid<Usuarios> GridUsu;
        protected SfGrid<Rol> GridRolesUsuarios;
        protected List<Usuarios> lUsuarios;
        protected List<Rol> lRoles;
        protected Rol RolSeleccionado;
        protected Usuarios usuario = new();
        protected async override Task OnInitializedAsync()
        {
            //await SpinnerObj.ShowAsync();
            await GetUsuarios();
            await GetRoles();
            //await SpinnerObj.HideAsync();
        }

        private async Task GetRoles()
        {
            var responseWrapper = await Http.Get<List<Rol>>("api/Usuarios/roles");
            if (responseWrapper.Error)
            {
                //await SpinnerObj.HideAsync();
                var mensajeServidor = responseWrapper.GetBody();
                Console.WriteLine(mensajeServidor);
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "Error!",
                    Content = "",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons"
                });
            }
            else
            {
                lRoles = responseWrapper.Response;
            }
        }

        private async Task GetUsuarios()
        {
            var responseWrapper = await Http.Get<List<SupplyChain.Shared.Models.Usuarios>>("api/Usuarios");
            if (responseWrapper.Error)
            {
                //await SpinnerObj.HideAsync();
                var mensajeServidor = responseWrapper.GetBody();
                Console.WriteLine(mensajeServidor);
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "Error!",
                    Content = "",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons"
                });
            }
            else
            {
                lUsuarios = responseWrapper.Response;
            }
        }

        protected async Task AsignarRol()
        {
            if (RolSeleccionado == null)
                return;

            var rolUser = new RolUsuario()
            {
                RolId = RolSeleccionado.Id,
                UserId = usuario.Id
            };

            var response = await Http.Post<RolUsuario>("api/Usuarios/asignarRol", rolUser);
            if (response.Error)
            {
                var mensajeServidor = await response.GetBody();
                Console.WriteLine(mensajeServidor);
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "Error!",
                    Content = "",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowProgressBar = true,
                    ShowCloseButton = true
                });
            }
            else
            {
                usuario.Roles.Add(RolSeleccionado);
                await GridRolesUsuarios.RefreshColumns();
                GridRolesUsuarios.Refresh();
                await GridRolesUsuarios.RefreshHeader();

            }
        }
        protected async Task ActionComplete(ActionEventArgs<Usuarios> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {

                var respuesta = await Http.Put($"api/Usurios/{args.Data.Id}", args.Data);
                if (respuesta.Error)
                {
                    await ToastObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = $"Ocurrrio un error.Error al intentar Guardar Usuario: {args.Data.Nombre} ",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
                else
                {
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "Exito!",
                        Content = $"Guardado Correctamente! Nro OF: {args.Data.Nombre}",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });


                    await GridUsu.RefreshColumns();
                    GridUsu.Refresh();
                    await GridUsu.RefreshHeader();
                }
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.RowDragAndDrop)
            {

            }
        }

        protected async Task OnSelected(Syncfusion.Blazor.DropDowns.SelectEventArgs<Rol> args)
        {
            RolSeleccionado = args.ItemData;
        }
    }
}
