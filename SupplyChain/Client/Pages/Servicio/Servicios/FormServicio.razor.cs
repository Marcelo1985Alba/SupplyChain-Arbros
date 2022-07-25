using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Pages.Ventas._3_Presupuestos;
using SupplyChain.Client.Pages.Ventas._4_Solicitudes;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Servicio.Servicios
{
    public class FormServicioBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public CeldasService CeldasService { get; set; }
        [Inject] public ClienteService ClienteService { get; set; }
        [Inject] public SolicitudService SolicitudService { get; set; }
        [Inject] public PresupuestoService PresupuestoService { get; set; }
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public bool Disabled { get; set; } = true;
        [Parameter] public Service Servicio { get; set; } = new();
        [Parameter] public EventCallback<Service> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected class SIoNO
        { 
            public string Text { get; set; }
        }
        protected List<SIoNO> SIoNOData = new List<SIoNO> {
            new SIoNO() {Text= "SI"},
            new SIoNO() {Text= "NO"}
        };

        public class Tipos
        {
            public string Text { get; set; }
        }
        public List<Tipos> TipoData = new List<Tipos> {
            new Tipos() {Text= "Cte"},
            new Tipos() {Text= "VAR"}
        };

        public class Estados
        {
            public string Text { get; set; }
        }
        public List<Estados> EstadosData = new List<Estados> {
            new Estados() {Text= "BUENO"},
            new Estados() {Text= "REGULAR"},
            new Estados() {Text= "MUY DETERIORADO"}
        };

        public class Sobrepresiones
        {
            public string Text { get; set; }
        }
        public List<Sobrepresiones> SobrepresionData = new List<Sobrepresiones> {
            new Sobrepresiones() {Text= "3"},
            new Sobrepresiones() {Text= "10"},
            new Sobrepresiones() {Text= "16"},
            new Sobrepresiones() {Text= "21"},
            new Sobrepresiones() {Text= "25"}
        };

        protected List<Solution> rutas = new();
        protected IEnumerable<Operario> opers;
        protected List<Medida> medidas = new List<Medida>();
        protected List<Serie> series = new List<Serie>();
        protected List<Orificio> orificios = new List<Orificio>();
        protected List<Sobrepresion> sobrepresiones = new List<Sobrepresion>();
        protected List<Tipo> tipos = new List<Tipo>();
        protected List<Estado> estados = new List<Estado>();
        protected List<Trabajosefec> trabajosEfectuados = new List<Trabajosefec>();
        protected List<Marca> marcas = new List<Marca>();
        protected List<Operario> operarios = new List<Operario>();
        protected List<Celdas> celdas = new List<Celdas>();
        protected List<Producto> prods = new List<Producto>();

        protected Dictionary<string, object> presupuestoButton = new Dictionary<string, object>()
        {
            { "title", "Generar Presupuesto"}
        };

        protected Dictionary<string, object> pedidoButton = new Dictionary<string, object>()
        {
            { "title", "Generar Pedido"}
        };

        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" },
            { "form", "formServicio" }

        };
        protected FormPresupuesto refFormPresupuesto;
        protected bool verDialogPresupuesto = false;
        protected Presupuesto presupuesto = new();

        protected FormSolicitud refFormSolicitud;
        protected bool verDialogSolicitud = false;
        protected Solicitud solicitud = new();
        protected async override Task OnInitializedAsync()
        {
            medidas = await Http.GetFromJsonAsync<List<Medida>>("api/Medida");
            series = await Http.GetFromJsonAsync<List<Serie>>("api/Serie");
            orificios = await Http.GetFromJsonAsync<List<Orificio>>("api/Orificio");
            sobrepresiones = await Http.GetFromJsonAsync<List<Sobrepresion>>("api/Sobrepresion");
            tipos = await Http.GetFromJsonAsync<List<Tipo>>("api/Tipo");
            estados = await Http.GetFromJsonAsync<List<Estado>>("api/Estado");
            trabajosEfectuados = await Http.GetFromJsonAsync<List<Trabajosefec>>("api/TrabajosEfec");
            marcas = await Http.GetFromJsonAsync<List<Marca>>("api/Marca");
            operarios = await Http.GetFromJsonAsync<List<Operario>>("api/Operario");
            opers = from opers in (IEnumerable<Operario>)operarios
                    where opers.ACTIVO == true
                    select opers;
            var response = await CeldasService.Get();
            if (!response.Error)
            {
                celdas = response.Response;
            }
            //celdas = await Http.GetFromJsonAsync<List<Celdas>>("api/Celdas");
            rutas = await Http.GetFromJsonAsync<List<Solution>>("api/Solution");
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
            solicitud = new();
            if (Servicio.SOLICITUD > 0)
            {
                await GetSolicitud();
            }

            verDialogSolicitud = true;

        }


        protected async Task<ClienteExterno> GetCliente()
        {
            var response = await ClienteService.GetClientesExternoByCg_Cli(Servicio.CG_CLI);
            if (response.Error)
            {
                //TODO: MOSTRAR ERROR
                return new ClienteExterno();
            }
            else
            {
                return response.Response;
            }

        }

        protected async Task<Presupuesto> GetPresupuesto()
        {
            var response = await PresupuestoService.GetById(Servicio.PRESUPUESTO);
            if (response.Error)
            {
                //TODO: MOSTRAR ERROR
                return new Presupuesto();
            }
            else
            {
                return response.Response;
            }

        }

        protected async Task MostrarPresupuesto()
        {
            presupuesto = new();
            if (Servicio.PRESUPUESTO == 0)
            {
                var clienteExterno = await GetCliente();

                presupuesto.CG_CLI = Servicio.CG_CLI;
                presupuesto.DES_CLI = clienteExterno.DESCRIPCION.Trim();
                presupuesto.CONDICION_PAGO = clienteExterno.ID_CON_VEN == null ? 0 : (int)clienteExterno.ID_CON_VEN;//hay cliente que no tienen asignado una condicion de pago
                presupuesto.BONIFIC = clienteExterno.DESC_COMERCIAL == null ? 0 : (decimal)clienteExterno.DESC_COMERCIAL;
                presupuesto.CG_COND_ENTREGA = clienteExterno.ID_CON_ENT == null ? 0 : (int)clienteExterno.ID_CON_ENT;
                await refFormPresupuesto.ClienteExternoSelected(clienteExterno);

                PresupuestoDetalle item = new();
                if (solicitud.Id == 0)
                {
                    await GetSolicitud();
                }
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
                
                if (OnGuardar.HasDelegate)
                {
                    await OnGuardar.InvokeAsync(Servicio);
                }
                verDialogPresupuesto = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task Guardar()
        {
            if (OnGuardar.HasDelegate)
            {
                await OnGuardar.InvokeAsync(Servicio);
                await OnCerrar.InvokeAsync();
            }
        }

        protected async Task OnCerrarDialog()
        {
            Show = false;
            await OnCerrar.InvokeAsync();
        }
    }
}
