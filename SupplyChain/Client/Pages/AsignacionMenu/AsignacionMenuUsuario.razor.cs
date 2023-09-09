using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.AsignacionMenu;

public class AsignacionMenuUsuarioBase : ComponentBase
{
    private AuthenticationState authState;
    public string[] CheckedNodes = { };
    protected List<Modulo> dbMenu = new();
    protected List<Modulo> dbMenuMaster = new();
    protected ExpandAction Expand = ExpandAction.Click;
    protected bool IsAdmin = false;
    protected bool IsCompras = false;
    protected bool IsIngenieria = false;
    protected bool IsProd = false;
    protected bool IsVentas = false;

    protected SfSpinner refSpinner;
    protected bool SpinnerVisibleMenuMaestro;
    protected SfToast ToastObj;
    protected SfTreeView<Modulo>? treeviewMaster;
    protected SfTreeView<Modulo>? treeviewUsuario;
    protected List<vUsuario> Usuarios = new();
    [Inject] public IRepositoryHttp Http { get; set; }
    [Inject] public ModulosUsuarioService ModulosUsuarioService { get; set; }

    [CascadingParameter] public MainLayout Layout { get; set; }
    [CascadingParameter] public Task<AuthenticationState> authenticationState { get; set; }
    protected string ChangeIdUsuario { get; set; } = "";

    protected override async Task OnInitializedAsync()
    {
        Layout.Titulo = "Asignacion de Menu por Usuario";
        // To get the last item index from the db
        await GetMenu();
        await GetUsuarios();
    }

    protected async Task GetUsuarios()
    {
        var response = await Http.GetFromJsonAsync<List<vUsuario>>("api/Cuentas/Usuarios");
        if (response.Error)
            Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
        else
            Usuarios = response.Response;
    }

    protected async Task GetMenu()
    {
        SpinnerVisibleMenuMaestro = true;
        authState = await authenticationState;
        if (authState.User.Identity.IsAuthenticated)
        {
            var response = await Http.GetFromJsonAsync<List<Modulo>>("api/Modulo");
            if (response.Error)
                await ToastMensajeError("Error al obtener Menu Maestro");
            else
                dbMenuMaster = response.Response;
        }

        SpinnerVisibleMenuMaestro = false;
    }

    protected async Task GetMenuUsuario()
    {
        var response = await ModulosUsuarioService.GetModulosFomUserId(ChangeIdUsuario);
        if (response.Error)
        {
            await ToastMensajeError("Error al obtener Modulos del usuario");
        }
        else
        {
            dbMenu.AddRange(response.Response.Except(dbMenu));
            await treeviewUsuario.Refresh();
        }
    }

    public void CheckAll()
    {
        // To check all the nodes in the TreeView
        treeviewMaster.CheckAllAsync();
        // To check the particular node in the TreeView
        // this.treeview.CheckAllAsync(new string[]{"1"});
    }

    public void UncheckAll()
    {
        // To uncheck all the nodes in the TreeView
        treeviewMaster.UncheckAllAsync();
        // To uncheck the particular node in the TreeView
        // this.treeview.UncheckAllAsync(new string[]{"1"});
    }

    protected async Task Guardar()
    {
        var listaModulosUsuarios = new List<ModulosUsuario>();

        listaModulosUsuarios = dbMenu.Select(s => new ModulosUsuario { ModuloId = s.Id, UserId = ChangeIdUsuario })
            .ToList();

        var response = await ModulosUsuarioService.GuardarLista(listaModulosUsuarios);
        if (response.Error)
            await ToastMensajeError("Error al guardar Menu");
        else
            await ToastMensajeExito();
    }

    public void NodeCheckedHandler(NodeCheckEventArgs args)
    {
        if (CheckedNodes is not null && CheckedNodes.Count() > 0)
        {
            //Obtener todos los seleccionados
            var nodes = treeviewMaster.GetAllCheckedNodes();
        }
    }

    protected async Task AgregarItemsSeleccionados(MouseEventArgs args)
    {
        //validaciones
        if (string.IsNullOrEmpty(ChangeIdUsuario))
        {
            await ToastMensajeError("Seleccione Usuario");
            return;
        }


        //Obtener todos los seleccionados
        var idNodes = treeviewMaster.GetAllCheckedNodes();
        if (idNodes is null || idNodes.Count == 0)
        {
            await ToastMensajeError("Seleccione menu");
            return;
        }


        if (idNodes is not null && idNodes.Count > 0)
        {
            //agregar al menu de usuario
            var modulosSeleccionados = dbMenuMaster.Where(m => idNodes.Select(int.Parse).Contains(m.Id)).ToList();

            var idModulosPadres = modulosSeleccionados.Where(w => w.ParentId.HasValue && w.ParentId.Value > 0)
                .Select(s => s.ParentId).ToList();


            //agregar al menu de usuario los padres no seleccionados
            var padreModulosSeleccionados = dbMenuMaster.Where(m =>
                !modulosSeleccionados.Select(s => s.Id).Contains(m.Id) &&
                idModulosPadres.Contains(m.Id)).ToList();


            var moduloUnicos = dbMenu.Concat(modulosSeleccionados).GroupBy(m => m.Id)
                .Select(s => s.FirstOrDefault()).ToList();
            dbMenu.AddRange(moduloUnicos.Except(dbMenu));


            if (padreModulosSeleccionados is not null && padreModulosSeleccionados.Count > 0)
                dbMenu.AddRange(padreModulosSeleccionados.Except(dbMenu));


            treeviewUsuario.Refresh();
        }
    }

    public async Task OnChange(ChangeEventArgs<string, vUsuario> args)
    {
        ChangeIdUsuario = args.ItemData.Id;

        if (!string.IsNullOrEmpty(ChangeIdUsuario))
        {
            dbMenu = new List<Modulo>();
            await GetMenuUsuario();
        }
    }

    private async Task ToastMensajeExito()
    {
        await ToastObj.ShowAsync(new ToastModel
        {
            Title = "EXITO!",
            Content = "Guardado Correctamente.",
            CssClass = "e-toast-success",
            Icon = "e-success toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true,
            NewestOnTop = true
        });
    }

    private async Task ToastMensajeError(string content = "Error al agregar Menu.")
    {
        await ToastObj.ShowAsync(new ToastModel
        {
            Title = "Error!",
            Content = content,
            CssClass = "e-toast-warning",
            Icon = "e-warning toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true,
            NewestOnTop = true
        });
    }
}