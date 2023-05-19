﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.RichTextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Compras.SolicitudCotizacion
{
    public class EmailPreviewBase: ComponentBase
    {
        [Inject] public IRepositoryHttp Http { get; set; }
        [Parameter] public List<SolCotEmail> EmailsEnviar { get; set; } = new();
        [Parameter] public EventCallback<List<SolCotEmail>> OnMailEnviadosCorrectamente { get; set; }

        protected bool dialogVisible =false;
        protected SfRichTextEditor richTextEditorRef;
        public SfToast ToastObj;

        protected string asunto = "Solicitud de Cotización";
        protected string mensaje = "Estimado/a [CONTACTO],\n" +
            "[CODIGO PROVEEDOR] - [PROVEEDOR]\n\n" +
            "Solicito cotización para los insumos:\n" +
            "[INSUMOS]";


        protected void VistaPreviaEmail(MouseEventArgs args)
        {
            ActualizarDatosEmailsAEnviar();
            dialogVisible = true;
        }
        protected void ActualizarDatosEmailsAEnviar()
        {
            string mensajeInsumos = string.Empty;
            foreach (var mail in EmailsEnviar)
            {
                
                mail.ASUNTO_EMAIL = asunto;
                mail.MENSAJE_EMAIL = mensaje.Replace("[CONTACTO]", mail.CONTACTO.Trim())
                    .Replace("[CODIGO PROVEEDOR]", mail.CG_PROVE.ToString())
                    .Replace("[PROVEEDOR]", mail.Proveedor.Trim());

            }

            //Obtengo las mp no repetidas para generar el mensaje de email
            foreach (var mail in EmailsEnviar.GroupBy(g=> g.CG_MAT).Select(s=> s.FirstOrDefault()).ToList())
            {
                mensajeInsumos += $"Código: {mail.CG_MAT} Descripción: {mail.NombreInsumo} " +
                    $"Cantidad: {mail.CANTIDAD} {mail.UNIDAD}\n";
            }
            

            foreach (var mail in EmailsEnviar)
            {
                mail.MENSAJE_EMAIL = mail.MENSAJE_EMAIL.Replace("[INSUMOS]", mensajeInsumos);
            }
        }
        protected async Task EnviarMail(MouseEventArgs args)
        {

            ActualizarDatosEmailsAEnviar();
            var response = await Http.PostAsJsonAsync<List<SolCotEmail>>("api/SolCotEmail/EnviarMail", EmailsEnviar);
            if (response.Error)
            {
                Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Ocurrio un Error al enviar mails",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
            else
            {
                if (OnMailEnviadosCorrectamente.HasDelegate)
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = "Mails Enviados Correctamente",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = false,
                        ShowProgressBar = false
                    });
                    await OnMailEnviadosCorrectamente.InvokeAsync(response.Response);
                }
            }
        }

        public void ActualizarListaMails(List<SolCotEmail> emails)
        {
            EmailsEnviar = emails;
        }
    }
}
