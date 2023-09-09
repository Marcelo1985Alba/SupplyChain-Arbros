using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;

namespace SupplyChain.Server.Controllers;

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
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();
            if (roleClaims.Any(c => c.Value == "Cliente"))
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByNameAsync(userName);
                var nrclte_usuario = user.Cg_Cli;
                return await _context.vESTADOS_COMPRAS.Where(p => p.NROCLTE == nrclte_usuario).ToListAsync();
            }

            var estados = await _context.vESTADOS_COMPRAS.ToListAsync();

            return estados;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error:{e.Message}");
            throw;
            //return new List<vESTADOS_COMPRAS>();
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
        switch (estado)
        {
            case EstadoCompras.TodosPendientes:
                return await _context.vESTADOS_COMPRAS.Where(c => c.ESTADOS_COMPRA.ToUpper() != "CERRADA"
                                                                  && c.ESTADOS_COMPRA.ToUpper() != "PAGADA-RECIBIDA")
                    .ToListAsync();
            case EstadoCompras.Todos:
                return await _context.vESTADOS_COMPRAS.ToListAsync();
            default:
                return await _context.vESTADOS_COMPRAS.ToListAsync();
        }
        //if (estado == EstadoCompras.PendEmSolCot)
        //{
        //    return await _context.vESTADOS_COMPRAS.Where(e => e.ESTADOS_COMPRA == estado.ToString() && string.IsNullOrEmpty(e.REMITO))
        //        .ToListAsync();
        //}
        //    else if(estado == EstadoCompras.PendEmisionOC)
        //{
        //    return await _context.vESTADOS_COMPRAS.Where(e => !string.IsNullOrEmpty(e.REMITO))
        //        .ToListAsync();
        //}
        //else if(estado== EstadoCompras.PendEntFecha)
        //{
        //    return await _context.vESTADOS_COMPRAS.Where(e => !string.IsNullOrEmpty(e.REMITO))
        //        .ToListAsync();
        //}
        //else if (estado != EstadoCompras.PendEntVenc)
        //{
        //    return await _context.vESTADOS_COMPRAS.Where(e => e.ESTADOS_COMPRA == estado.ToString())
        //        .ToListAsync();
        //}
        //else if (estado !=EstadoCompras.RecParcialPendPago)
        //{
        //    return await _context.vESTADOS_COMPRAS.Where(e => e.ESTADOS_COMPRA==estado.ToString())
        //        .ToListAsync();
        //}
        //else if (estado == EstadoCompras.RecTotalPendPago)
        //{
        //    return await _context.vESTADOS_COMPRAS.Where(e => e.ESTADOS_COMPRA == estado.ToString())
        //        .ToListAsync();
        //}
        //else if (estado == EstadoCompras.PagRecibida)
        //{
        //    return await _context.vESTADOS_COMPRAS.Where(e => !string.IsNullOrEmpty(e.REMITO))
        //        .ToListAsync();
        //}
        //else if(estado == EstadoCompras.Cerrada)
        //{
        //    return await _context.vESTADOS_COMPRAS.Where(e => !string.IsNullOrEmpty(e.REMITO))
        //        .ToListAsync();
        //}
        //else
        //{
        //    return await _context.vESTADOS_COMPRAS.ToListAsync();
        //}
    }
}