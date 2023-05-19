using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SupplyChain.Server.Repositorios
{
    public class MailRepository
    {
        private readonly ConfigEmailCompras _configEmailCompras;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MailRepository(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configEmailCompras = configuration.GetSection("ConfigEmailCompras").Get<ConfigEmailCompras>();
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task EnviarCorreo(string destinatario, string asunto, string cuerpo)
        {
            
            var fromAddress = new MailAddress(_configEmailCompras.NombreUsuario, "Compras Aerre");
            var toAddress = new MailAddress(destinatario);
            //destinatario = _configEmailCompras.To;
            //var toAddress = new MailAddress(destinatario);
            var ccAddress = new MailAddress(_configEmailCompras.Copia);
            if (_webHostEnvironment.IsDevelopment())
            {
                toAddress = new MailAddress("m.albarracin@live.com.ar");
            }
            string fromPassword = _configEmailCompras.Contraseña;
            string smtpServer = _configEmailCompras.ServidorSmtp;
            int smtpPort = _configEmailCompras.Puerto;

            var smtp = new SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                EnableSsl = _configEmailCompras.RequiereAutenticacion,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = asunto,
                Body = cuerpo, 
                
            };

            message.CC.Add(ccAddress);

            await smtp.SendMailAsync(message);
        }
    }
}
