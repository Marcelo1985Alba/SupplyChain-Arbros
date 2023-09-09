using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SupplyChain.Shared;

namespace SupplyChain.Server.Repositorios;

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

        if (_webHostEnvironment.IsDevelopment()) toAddress = new MailAddress("m.albarracin@live.com.ar");
        var fromPassword = _configEmailCompras.Contraseña;
        var smtpServer = _configEmailCompras.ServidorSmtp;
        var smtpPort = _configEmailCompras.Puerto;

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

        foreach (var item in _configEmailCompras.Copia)
        {
            var ccAddress = new MailAddress(item);
            message.CC.Add(ccAddress);
        }


        await smtp.SendMailAsync(message);
    }
}