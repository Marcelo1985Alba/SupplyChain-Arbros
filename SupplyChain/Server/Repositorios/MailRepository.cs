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

namespace SupplyChain.Server.Repositorios
{
    public class MailRepository
    {
        private readonly ConfigEmailCompras _configEmailCompras;
        public MailRepository(IConfiguration configuration)
        {
            _configEmailCompras = configuration.GetSection("ConfigEmailCompras").Get<ConfigEmailCompras>();
        }

        public async Task EnviarCorreo(string destinatario, string asunto, string cuerpo)
        {
            
            var fromAddress = new MailAddress(_configEmailCompras.NombreUsuario, "Compras Aerre");
            //var toAddress = new MailAddress(destinatario);
            destinatario = _configEmailCompras.To;
            var toAddress = new MailAddress(destinatario);
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
                Body = cuerpo
            };

            await smtp.SendMailAsync(message);
        }
    }
}
