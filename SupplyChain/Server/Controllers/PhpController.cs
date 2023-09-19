using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupplyChain.Server.Hubs;
using SupplyChain.Shared;
using SupplyChain.Shared.Login;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhpController : ControllerBase
    {
        private HttpClient httpClient;
        private readonly AppDbContext _context;

        public PhpController(AppDbContext context)
        {
            this._context = context;

        }

        [HttpGet]
        public async Task<ActionResult> EnviarSolicitudPost([FromQuery] string fileName, string operacion, string serialPort)
        {
            httpClient = new HttpClient();
            // Datos JSON que deseas enviar
            var data = new
            {
                operation = operacion,
                filename = fileName,
                serialport = serialPort
            };

            // URL del servidor y script PHP
            var apiUrl = "http://192.168.0.131:8080/autentio/upload.php";

            try
            {
                // Realiza la solicitud POST
                var response = await httpClient.PostAsJsonAsync(apiUrl, data);

                // Verifica si la respuesta es exitosa
                if (response.IsSuccessStatusCode)
                {
                    var me = response.ReasonPhrase;
                    var res = await response.Content.ReadAsStringAsync();
                    // Procesa la respuesta aquí si es necesario
                    return Ok(res);
                    // Donde "TuTipoDeRespuesta" es la clase que representa la respuesta JSON del servidor
                }
                else
                {
                    return BadRequest("Error");
                }
            }
            catch (HttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
