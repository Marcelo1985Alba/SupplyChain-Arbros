using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using Syncfusion.XlsIO;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EstadoComprasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public EstadoComprasController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<vESTADOS_COMPRAS>>> Get()
        {
            try
            {
                List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();
                if (roleClaims.Any(c => c.Value == "Cliente"))
                {
                    var userName = HttpContext.User.Identity.Name;
                    var user = await userManager.FindByNameAsync(userName);
                    var nrclte_usuario = user.Cg_Cli;
                    return await _context.vESTADOS_COMPRAS.Where(p => p.NROCLTE == nrclte_usuario).ToListAsync();

                }
                else
                {
                    var estados = await _context.vESTADOS_COMPRAS.ToListAsync();

                    return estados;

                }

            }
            catch (Exception e)
            {
                return new List<vESTADOS_COMPRAS>();
            }

        } 

        //[HttpGet("GetRemitos{remito}")]
        //public async Task<ActionResult<vESTADOS_COMPRAS>> GetRemitos(string remito)
        //{
        //  var DRemito = await _context.GetRemitos(remito);
         
        //  return DRemito;
        //}

        //  1.Solicitar cotizacion
        //2.Pendiente de generar compra
        //3.A la esperad de cotizacion
        //4.Pendiente de entrega
        //5.Pagada
        //6.Vencida
        //7.Cerrada

        [HttpGet("ByEstado/{estado}")]
        public async Task<ActionResult<IEnumerable<vESTADOS_COMPRAS>>> Get(EstadoCompras estado = EstadoCompras.Todos)
        {
            if (estado == EstadoCompras.SolicitarCotizacion)
            {
                return await _context.vESTADOS_COMPRAS.Where(e => e.ESTADOS_COMPRA == estado.ToString() && string.IsNullOrEmpty(e.REMITO))
                    .ToListAsync();
            }
            else if(estado == EstadoCompras.PendienteGenerarCompra)
            {
                return await _context.vESTADOS_COMPRAS.Where(e => !string.IsNullOrEmpty(e.REMITO))
                    .ToListAsync();
            }
            else if(estado== EstadoCompras.AEsperaCotizacion)
            {
                return await _context.vESTADOS_COMPRAS.Where(e => !string.IsNullOrEmpty(e.REMITO))
                    .ToListAsync();
            }
            else if (estado != EstadoCompras.PendienteEntrega)
            {
                return await _context.vESTADOS_COMPRAS.Where(e => e.ESTADOS_COMPRA == estado.ToString())
                    .ToListAsync();
            }
            else if (estado == EstadoCompras.Pagada)
            {
                return await _context.vESTADOS_COMPRAS.Where(e => !string.IsNullOrEmpty(e.REMITO))
                    .ToListAsync();
            }
            else if(estado == EstadoCompras.Vencida)
            {
                return await _context.vESTADOS_COMPRAS.Where(e => !string.IsNullOrEmpty(e.REMITO))
                    .ToListAsync();
            }
            else if(estado == EstadoCompras.Cerrada)
            {
                return await _context.vESTADOS_COMPRAS.Where(e => !string.IsNullOrEmpty(e.REMITO))
                    .ToListAsync();
            }

            else if (estado != EstadoCompras.SolicitarCotizacion || estado != EstadoCompras.Todos)
            {
                return await _context.vESTADOS_COMPRAS.Where(e => e.ESTADOS_COMPRA == estado.ToString())
                    .ToListAsync();
            }

            else
            {
                return await _context.vESTADOS_COMPRAS.ToListAsync();
            }
        }
    }
} 
