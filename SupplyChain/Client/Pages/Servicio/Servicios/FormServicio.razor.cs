using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Pages.Ventas._3_Presupuestos;
using SupplyChain.Client.Pages.Ventas._4_Solicitudes;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Notifications;

namespace SupplyChain.Client.Pages.Servicio.Servicios;

public class FormServicioBase : ComponentBase
{
    protected List<Celdas> celdas = new();
    protected List<Estado> estados = new();

    public List<Estados> EstadosData = new()
    {
        new Estados() { Text = "BUENO" },
        new Estados() { Text = "REGULAR" },
        new Estados() { Text = "MUY DETERIORADO" }
    };

    protected Dictionary<string, object> HtmlAttributeSubmit = new()
    {
        { "type", "submit" },
        { "form", "formServicio" }
    };

    protected List<Marca> marcas = new();
    protected List<Medida> medidas = new();
    protected List<Operario> operarios = new();
    protected IEnumerable<Operario> opers;
    protected List<Orificio> orificios = new();

    protected Dictionary<string, object> pedidoButton = new()
    {
        { "type", "submit" },
        { "title", "Generar Pedido" }
    };

    protected Presupuesto presupuesto = new();

    protected Dictionary<string, object> presupuestoButton = new()
    {
        { "type", "button" },
        { "title", "Generar Presupuesto" }
    };

    protected List<Producto> prods = new();
    protected FormPresupuesto refFormPresupuesto;

    protected FormSolicitud refFormSolicitud;

    protected List<Solution> rutas = new();
    protected List<Serie> series = new();

    protected List<SIoNO> SIoNOData = new()
    {
        new SIoNO() { Text = "SI" },
        new SIoNO() { Text = "NO" }
    };

    public List<Sobrepresiones> SobrepresionData = new()
    {
        new Sobrepresiones() { Text = "3" },
        new Sobrepresiones() { Text = "10" },
        new Sobrepresiones() { Text = "16" },
        new Sobrepresiones() { Text = "21" },
        new Sobrepresiones() { Text = "25" }
    };

    protected List<Sobrepresion> sobrepresiones = new();
    protected Solicitud solicitud = new();
    protected bool spinnerVisibleGuardar;

    public List<Tipos> TipoData = new()
    {
        new Tipos() { Text = "Cte" },
        new Tipos() { Text = "VAR" }
    };

    protected List<Tipo> tipos = new();

    protected SfToast ToastObj;
    protected List<Trabajosefec> trabajosEfectuados = new();
    protected bool verDialogPresupuesto;
    protected bool verDialogSolicitud;
    [Inject] public IJSRuntime Js { get; set; }
    [Inject] public IRepositoryHttp Http2 { get; set; }
    [Inject] protected HttpClient Http { get; set; }
    [Inject] public CeldasService CeldasService { get; set; }
    [Inject] public ServicioService ServicioService { get; set; }
    [Inject] public ClienteService ClienteService { get; set; }
    [Inject] public SolicitudService SolicitudService { get; set; }
    [Inject] public PresupuestoService PresupuestoService { get; set; }
    [Parameter] public bool Show { get; set; }
    [Parameter] public List<Service> Servicios { get; set; } = new();
    [Parameter] public bool Disabled { get; set; } = true;
    [Parameter] public Service Servicio { get; set; } = new();
    [Parameter] public EventCallback<Service> OnGuardar { get; set; }
    [Parameter] public EventCallback OnCerrar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Servicios.Count < 0) await GetServicios();

        medidas = await Http.GetFromJsonAsync<List<Medida>>("api/Medida");
        series = await Http.GetFromJsonAsync<List<Serie>>("api/Serie");
        orificios = await Http.GetFromJsonAsync<List<Orificio>>("api/Orificio");
        sobrepresiones = await Http.GetFromJsonAsync<List<Sobrepresion>>("api/Sobrepresion");
        tipos = await Http.GetFromJsonAsync<List<Tipo>>("api/Tipo");
        estados = await Http.GetFromJsonAsync<List<Estado>>("api/Estado");
        trabajosEfectuados = await Http.GetFromJsonAsync<List<Trabajosefec>>("api/TrabajosEfec");
        marcas = await Http.GetFromJsonAsync<List<Marca>>("api/Marca");
        operarios = await Http.GetFromJsonAsync<List<Operario>>("api/Operario");
        opers = from opers in operarios
            where this.opers.ACTIVO
            select opers;
        var response = await CeldasService.Get();
        if (!response.Error) celdas = response.Response;
        //celdas = await Http.GetFromJsonAsync<List<Celdas>>("api/Celdas");
        rutas = await Http.GetFromJsonAsync<List<Solution>>("api/Solution");
    }

    private async Task GetServicios(TipoFiltro tipoFiltro = TipoFiltro.Todos)
    {
        var ApiUrl = "api/Servicios";
        //servicios = await Http.GetFromJsonAsync<List<Service>>("api/Servicios");
        var responseService = await ServicioService.GetByFilter(tipoFiltro);
        if (responseService.Error)
        {
        }
        else
        {
            Servicios = responseService.Response;
        }
    }

    protected async Task GetSolicitud()
    {
        var response = await SolicitudService.GetById(Servicio.SOLICITUD);
        if (response.Error)
        {
        }
        else
        {
            solicitud = response.Response;
        }
    }

    protected async Task MostrarSolicitud()
    {
        solicitud = new Solicitud();
        if (Servicio.SOLICITUD > 0) await GetSolicitud();

        verDialogSolicitud = true;
    }


    protected async Task<ClienteExterno> GetCliente()
    {
        var response = await ClienteService.GetClientesExternoByCg_Cli(Servicio.CG_CLI);
        if (response.Error)
            //TODO: MOSTRAR ERROR
            return new ClienteExterno();
        return response.Response;
    }

    protected async Task<Presupuesto> GetPresupuesto()
    {
        var response = await PresupuestoService.GetById(Servicio.PRESUPUESTO);
        if (response.Error)
            //TODO: MOSTRAR ERROR
            return new Presupuesto();
        return response.Response;

        //return new Presupuesto();
    }

    protected async Task MostrarPresupuesto()
    {
        presupuesto = new Presupuesto();
        if (Servicio.PRESUPUESTO == 0)
        {
            var clienteExterno = await GetCliente();

            presupuesto.CG_CLI = Servicio.CG_CLI;
            presupuesto.DES_CLI = clienteExterno.DESCRIPCION.Trim();
            presupuesto.CONDICION_PAGO =
                clienteExterno.ID_CON_VEN == null
                    ? 0
                    : (int)clienteExterno.ID_CON_VEN; //hay cliente que no tienen asignado una condicion de pago
            presupuesto.BONIFIC = clienteExterno.DESC_COMERCIAL == null ? 0 : (decimal)clienteExterno.DESC_COMERCIAL;
            presupuesto.CG_COND_ENTREGA = clienteExterno.ID_CON_ENT == null ? 0 : (int)clienteExterno.ID_CON_ENT;
            await refFormPresupuesto.ClienteExternoSelected(clienteExterno);

            PresupuestoDetalle item = new();
            if (solicitud.Id == 0) await GetSolicitud();
            presupuesto.DES_CLI = solicitud.Des_Cli;
            item.CG_ART = solicitud.Producto;
            item.DES_ART = solicitud.Des_Prod;
            item.CANTIDAD = solicitud.Cantidad;
            item.SOLICITUDID = solicitud.Id;
            item.PREC_UNIT = solicitud.PrecioArticulo.Precio;
            presupuesto.Items.Add(item);
            await refFormPresupuesto.GetTipoCambioDolarHoy();
            await refFormPresupuesto.ShowAsync(Servicio.PRESUPUESTO);
            refFormPresupuesto.HabilitarComboMoneda();
        }
        else
        {
            presupuesto = await GetPresupuesto();
            await refFormPresupuesto.ShowAsync(Servicio.PRESUPUESTO);
        }

        verDialogPresupuesto = true;
    }

    protected async Task OnGuardarPresupuesto(Presupuesto presupuestoGuardado)
    {
        if (presupuestoGuardado.GUARDADO)
        {
            presupuesto.Id = presupuestoGuardado.Id;
            Servicio.PRESUPUESTO = presupuesto.Id;

            if (OnGuardar.HasDelegate) await OnGuardar.InvokeAsync(Servicio);
            verDialogPresupuesto = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    protected async Task Guardar()
    {
        var actualizado = await ActualizarServicio();
        if (actualizado)
            if (OnGuardar.HasDelegate)
            {
                await OnGuardar.InvokeAsync(Servicio);
                await OnCerrar.InvokeAsync();
            }
    }


    protected async Task<bool> ActualizarServicio()
    {
        spinnerVisibleGuardar = true;
        var responsePedAnt = await Http2.GetFromJsonAsync<Service>($"api/Servicios/GetByPedido/{Servicio.PEDIDOANT}");

        if (responsePedAnt.Error)
        {
        }
        else
        {
            var pedidoAnterior = responsePedAnt.Response;

            var pedant = Servicios.FirstOrDefault(s => s.Id == Servicio.Id)?.PEDIDOANT;
            if (pedant != Servicio.PEDIDOANT && pedidoAnterior != null &&
                !string.IsNullOrEmpty(pedidoAnterior.PEDIDOANT))
            {
                var isConfirmed = await Js.InvokeAsync<bool>("confirm", "Quiere traer los datos del pedido anterior?");
                if (isConfirmed)
                {
                    if (pedidoAnterior.FECHA.ToString().Substring(3, 1) == "/")
                        Servicio.FECMANTANT = pedidoAnterior.FECHA.ToString().Substring(0, 8);
                    else if (pedidoAnterior.FECHA.ToString().Substring(4, 1) == "/")
                        Servicio.FECMANTANT = pedidoAnterior.FECHA.ToString().Substring(0, 9);
                    else if (pedidoAnterior.FECHA.ToString().Substring(5, 1) == "/")
                        Servicio.FECMANTANT = pedidoAnterior.FECHA.ToString().Substring(0, 10);
                    Servicio.IDENTIFICACION = pedidoAnterior.IDENTIFICACION;
                    Servicio.MARCA = pedidoAnterior.MARCA;
                    Servicio.NSERIE = pedidoAnterior.NSERIE;
                    Servicio.MODELO = pedidoAnterior.MODELO;
                    Servicio.MEDIDA = pedidoAnterior.MEDIDA;
                    Servicio.SERIE = pedidoAnterior.SERIE;
                    Servicio.ORIFICIO = pedidoAnterior.ORIFICIO;
                    Servicio.AÑO = pedidoAnterior.AÑO;
                    Servicio.AREA = pedidoAnterior.AREA;
                    Servicio.FLUIDO = pedidoAnterior.FLUIDO;
                    Servicio.SOBREPRESION = pedidoAnterior.SOBREPRESION;
                    Servicio.PRESION = pedidoAnterior.PRESION;
                    Servicio.CONTRAPRESION = pedidoAnterior.CONTRAPRESION;
                    Servicio.TIPO = pedidoAnterior.TIPO;
                    Servicio.TEMP = pedidoAnterior.TEMP;
                    Servicio.RESORTE = pedidoAnterior.RESORTE;
                    Servicio.PRESIONBANCO = pedidoAnterior.PRESIONBANCO;
                    Servicio.SERVICIO = pedidoAnterior.SERVICIO;
                }
            }
        }


        var response = await Http2.PutAsJsonAsync($"api/Servicios/{Servicio.Id}", Servicio);
        if (response.Error)
        {
            await ToastObj.Show(new ToastModel
            {
                Title = "ERROR!",
                Content = "Error al actualizar pedido",
                CssClass = "e-toast-danger",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });


            return false;
        }

        await ToastObj.Show(new ToastModel
        {
            Title = "EXITO!",
            Content = "Servico actualizado correctamente",
            CssClass = "e-toast-success",
            Icon = "e-success toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });


        return true;


        spinnerVisibleGuardar = false;
    }


    protected async Task OnCerrarDialog()
    {
        Show = false;
        await OnCerrar.InvokeAsync();
    }

    protected class SIoNO
    {
        public string Text { get; set; }
    }

    public class Tipos
    {
        public string Text { get; set; }
    }

    public class Estados
    {
        public string Text { get; set; }
    }

    public class Sobrepresiones
    {
        public string Text { get; set; }
    }
}