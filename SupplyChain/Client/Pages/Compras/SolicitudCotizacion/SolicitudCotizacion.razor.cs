using Microsoft.AspNetCore.Components;
using static SupplyChain.Client.Pages.ChatApp.ChatBase;
using System.Collections.Generic;
using Syncfusion.Blazor.Inputs;
using SupplyChain.Shared.Models;
using System.Threading.Tasks;
using System;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using System.Linq;
using System.Text.Json;
using SupplyChain.Client.Pages.Compras.SolicitudCotizacion;

namespace SupplyChain.Client.Pages.Compras
{
    public class SolicitudCotizacionBase: ComponentBase
    {
        [Inject] public IRepositoryHttp Http { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }

        protected bool mostrarSpinner = false;
        protected SugerenciasCompras refSugerenciasCompras;
        protected ProveedoresMateriaPrima refProveedoresMateriaPrima;
        protected Proveedores refProveedores;
        protected EmailPreview refEmailPreview;
        protected EmailsEnviados refEmailsEnviados;
        protected List<vProveedorItris> vProveedorItrisEnviarSolicitud = new();
        protected List<SolCotEmail> EmailsEnviados = new();
        protected List<SolCotEmail> EmailsEnviar = new();
        protected List<Compra> sugerenciasSeleccionadas = new();

        protected int[] idsProveedores = Array.Empty<int>();
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Solicitud de Cotizacion";
        }

        protected async Task GetProveedoresFromSugerencias(Compra[] sugerenciasSeleccionadas)
        {
            if (sugerenciasSeleccionadas is not null && sugerenciasSeleccionadas.Length > 0)
            {
                List<Compra> lista = sugerenciasSeleccionadas.ToList();
                var urlGetProveFromMatProve = $"api/MatProve/GetProveedores?sugerencias={JsonSerializer.Serialize(lista)}";
                var response = await Http.Post<List<Compra>, List<vProveedorItris>>("api/MatProve/GetProveedores", lista);

                if (response.Error)
                {
                    Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                }
                else
                {
                    idsProveedores = response.Response.Select(s => s.Id).ToArray();
                    EnviarProveedoresMp();
                }
            }
            else
            {
                idsProveedores = Array.Empty<int>();
                EnviarProveedoresMp();
            }
        }
        protected async Task OnRecibirItemsSeleccionados(Compra[] sugerenciasSeleccionadas)
        {
            mostrarSpinner = true;

            EmailsEnviar.Clear();
            vProveedorItrisEnviarSolicitud.Clear();
            EmailsEnviados.Clear();
            this.sugerenciasSeleccionadas.Clear();

            this.sugerenciasSeleccionadas = sugerenciasSeleccionadas.ToList();
            await GetProveedoresFromSugerencias(sugerenciasSeleccionadas);
            await GetEmailsEnviadosFromSugerencias(sugerenciasSeleccionadas);
            mostrarSpinner = false;
        }

        protected async Task GetEmailsEnviadosFromSugerencias(Compra[] sugerenciasSeleccionadas)
        {

            if (sugerenciasSeleccionadas is not null && sugerenciasSeleccionadas.Length > 0)
            {
                List<Compra> lista = sugerenciasSeleccionadas.ToList();
                var response = await Http.Post<List<Compra>, List<SolCotEmail>>("api/SolCotEmail/BySugerenciasCompras", lista);

                if (response.Error)
                {
                    Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                }
                else
                {
                    EmailsEnviados = response.Response;
                    EnviarProveedoresMp();
                }
            }
            else
            {
                EmailsEnviados = new();
            }
        }

        protected void EnviarProveedoresMp()
        {
            refProveedoresMateriaPrima.SetIntArray(idsProveedores);
            refProveedores.SetIntArray(idsProveedores);
        }

        protected void OnProveedorDeseleccionado(vProveedorItris proveedorItris)
        {
            //vProveedorItrisEnviarSolicitud.Remove(proveedorItris);
            vProveedorItrisEnviarSolicitud = vProveedorItrisEnviarSolicitud.Where(p=> p.Id != proveedorItris.Id).ToList();
            ActualizarListaEmailsAEnviar();
        }
        protected void OnProveedorSeleccionado(vProveedorItris proveedorItris)
        {
            vProveedorItrisEnviarSolicitud.Add(proveedorItris);
            ActualizarListaEmailsAEnviar();
        }

        /// <summary>
        /// preparacion de los mails
        /// </summary>
        protected void ActualizarListaEmailsAEnviar()
        {
            EmailsEnviar.Clear();

            foreach (var proveedor in vProveedorItrisEnviarSolicitud)
            {
                //se debe preparar un mail por proveedor
                var mensaje = string.Empty;
                var mail = new SolCotEmail();
                foreach (var sugerenciaCompra in sugerenciasSeleccionadas)
                {
                    mensaje += $"\nCódigo: {sugerenciaCompra.CG_MAT.Trim()}\nDescripción: {sugerenciaCompra.DES_MAT.Trim()}\n" +
                        $"Cantidad: {Math.Round(sugerenciaCompra.SOLICITADO.Value, 2)} {sugerenciaCompra.UNID}\n" +
                        $"Fecha Prevista: {sugerenciaCompra.FE_PREV.Value.ToString("dd/MM/yyyy")}\n";

                    if (!string.IsNullOrEmpty(sugerenciaCompra.ESPECIFICA))
                    {
                        mensaje += $"Especificación técnica: {sugerenciaCompra.ESPECIFICA}\n";
                    }

                    mensaje += "--------------------------------------------------------------------------";
                    //"----------------------------------------------------";

                    mail = new SolCotEmail()
                    {
                        CG_CIA = 1,
                        FE_SOLCOT = DateTime.Now,
                        CG_MAT = sugerenciaCompra.CG_MAT.Trim(),
                        NombreInsumo = $"{sugerenciaCompra.DES_MAT.Trim()}",
                        CANTIDAD = sugerenciaCompra.SOLICITADO.Value,
                        CG_PROVE = proveedor.Id,
                        Proveedor = proveedor.DESCRIPCION.Trim(),
                        EMAIL = proveedor.EMAIL_CONTACTO,
                        CONTACTO = proveedor.NOMBRE_CONTACTO,
                        UNIDAD = sugerenciaCompra.UNID,
                        FE_PREV = sugerenciaCompra.FE_PREV.Value,
                        REGISTRO_COMPRAS = sugerenciaCompra.Id,
                        ASUNTO_EMAIL = string.Empty,
                        MENSAJE_EMAIL = string.Empty,
                        USUARIO = string.Empty
                    };

                    EmailsEnviar.Add(mail);
                }


                //corrige el mensaje para todos los items
                foreach (var item in EmailsEnviar)
                {
                    item.MENSAJE_EMAIL = mensaje;
                }
                
            }

            refEmailPreview.ActualizarListaMails(EmailsEnviar);
        }

        protected async Task MailsEnviadosCorrectamente(List<SolCotEmail> mailsEnviados)
        {
            //mailsEnviados.AddRange(mailsEnviados);

            //refSugerenciasCompras.ActualizarItems(sugerenciasSeleccionadas);



            await refEmailsEnviados.Refrescar(mailsEnviados);
        }
    }
}
