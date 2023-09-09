﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Action = Syncfusion.Blazor.Grids.Action;

namespace SupplyChain.Client.Pages.ABM.Usuarios;

public class UsuariosPageBase : ComponentBase
{
    protected List<ClienteExterno> Clientes = new();
    protected bool popupFormVisible;
    protected SfGrid<vUsuario> refGrid;
    protected SfSpinner refSpinner;

    protected bool SpinnerVisible;
    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        //new ItemModel { Text = "Add", TooltipText = "Agregar un nuevo Usuario", PrefixIcon = "e-add", Id = "Add" },
        "Add",
        "Edit",
        "Delete",
        //new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
        "ExcelExport",
        new ItemModel { Text = "", TooltipText = "Actualizar Grilla", PrefixIcon = "e-refresh", Id = "refresh" }
    };

    protected List<vUsuario> Usuarios = new();
    protected ApplicationUser UsuarioSeleccionado = new();
    protected vUsuario vUsuarioSeleccionado = new();
    [Inject] private IRepositoryHttp Http { get; set; }
    [Inject] public IJSRuntime JS { get; set; }
    [Inject] public ClienteService ClienteService { get; set; }
    [CascadingParameter] public MainLayout MainLayout { get; set; }


    protected override async Task OnInitializedAsync()
    {
        SpinnerVisible = true;
        await GetUsuarios();
        await GetClientes();
        SpinnerVisible = false;
    }

    protected async Task<List<vUsuario>> GetUsuarios()
    {
        var response = await Http.GetFromJsonAsync<List<vUsuario>>("api/Cuentas/Usuarios");
        if (response.Error)
        {
            Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
            return new List<vUsuario>();
        }

        return Usuarios = response.Response;
    }

    protected async Task GetClientes()
    {
        var response = await ClienteService.GetClientesExterno();
        if (response.Error)
        {
        }
        else
        {
            Clientes = response.Response;
        }
    }

    protected async Task OnActionBeginHandler(ActionEventArgs<vUsuario> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
        }

        if (args.RequestType == Action.Add)
        {
            SpinnerVisible = true;
            UsuarioSeleccionado = new ApplicationUser();
            UsuarioSeleccionado.EsNuevo = true;
            popupFormVisible = true;
            SpinnerVisible = false;
        }

        if (args.RequestType == Action.BeginEdit)
        {
            SpinnerVisible = true;
            var response = await Http.GetFromJsonAsync<ApplicationUser>($"api/Cuentas/usuarios/byId/{args.RowData.Id}");
            if (response.Error)
            {
                //await ToastMensajeError();
            }
            else
            {
                UsuarioSeleccionado = response.Response;
                //direccionesEntregas = PedidoSeleccionado.DireccionesEntregas.Select(d => d.DESCRIPCION).ToList();
                popupFormVisible = true;
            }

            SpinnerVisible = false;
        }
    }

    protected async Task OnActionCompleteHandler(ActionEventArgs<vUsuario> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
        }
    }

    protected async Task OnGuardar(ApplicationUser applicationUser)
    {
        await ToastMensajeExito();
        if (applicationUser.EsNuevo)
        {
            var vUsuario = new vUsuario
            {
                Id = applicationUser.Id,
                CLIENTE = applicationUser.NombreCliente,
                EMAIL = applicationUser.Email,
                USUARIO = applicationUser.UserName,
                FOTO = applicationUser.Foto
            };

            Usuarios.Add(vUsuario);
        }
        else
        {
            var vUsuario = Usuarios.FirstOrDefault(u => u.Id == applicationUser.Id);
            vUsuario.CLIENTE = applicationUser.NombreCliente;
            vUsuario.EMAIL = applicationUser.Email;
            vUsuario.USUARIO = applicationUser.UserName;
            vUsuario.FOTO = applicationUser.Foto;
        }

        popupFormVisible = false;
        refGrid.Refresh();
    }


    private async Task ToastMensajeExito()
    {
        await ToastObj.Show(new ToastModel
        {
            Title = "EXITO!",
            Content = "Guardado Correctamente.",
            CssClass = "e-toast-success",
            Icon = "e-success toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
    }


    public async Task CommandClickHandler(CommandClickEventArgs<vUsuario> args)
    {
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Seleccionar Columnas")
        {
            await refGrid.OpenColumnChooser(200, 50);
        }
        else if (args.Item.Text == "Exportar grilla en Excel")
        {
            await refGrid.ExcelExport();
        }
        else if (args.Item.Id == "refresh")
        {
            await GetUsuarios();
        }
        //await GetPedidoPendientesRemitir();
    }
}