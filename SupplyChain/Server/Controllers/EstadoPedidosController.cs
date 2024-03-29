﻿using System;
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

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EstadoPedidosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public EstadoPedidosController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: api/Genera
        [HttpGet]   
        public async Task<ActionResult<IEnumerable<vEstadoPedido>>> Get()
        {
            List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();
            try
            {
                if (roleClaims.Any(c => c.Value == "Cliente"))
                {
                    var userName = HttpContext.User.Identity.Name;
                    var user = await userManager.FindByNameAsync(userName);
                    var cg_cli_usuario = user.Cg_Cli;
                    return await _context.vEstadoPedidos.Where(p => p.CG_CLI == cg_cli_usuario).ToListAsync();
                }

                return await _context.vEstadoPedidos.ToListAsync();

            }catch (Exception ex)
            {
                return new List<vEstadoPedido>();
                 
            }
        }

        
        [HttpGet("ByEstado/{estado}")]
        public async Task<ActionResult<IEnumerable<vEstadoPedido>>> Get(EstadoPedido estado = EstadoPedido.Todos)
        {

            switch (estado)
            {
                case EstadoPedido.PendienteRemitir:
                    return await _context.vEstadoPedidos.Where(e => e.ESTADO_PEDIDO == (int)estado && string.IsNullOrEmpty(e.REMITO))
                            .ToListAsync();
                case EstadoPedido.Entregado:
                    return await _context.vEstadoPedidos.Where(e => !string.IsNullOrEmpty(e.REMITO))
                            .ToListAsync();
                case EstadoPedido.TodosPendientes:
                    return await _context.vEstadoPedidos.Where(c => c.ESTADO_PEDIDO < 10).ToListAsync();
                case EstadoPedido.Todos:
                    return await _context.vEstadoPedidos.ToListAsync();
                default:
                    return await _context.vEstadoPedidos.ToListAsync();
                    
            }
            //if (estado == EstadoPedido.PendienteRemitir)
            //{
            //    return await _context.vEstadoPedidos.Where(e => e.ESTADO_PEDIDO == (int)estado && string.IsNullOrEmpty(e.REMITO))
            //    .ToListAsync();
            //}
            //else if (estado == EstadoPedido.Entregado)
            //{
            //    return await _context.vEstadoPedidos.Where(e => !string.IsNullOrEmpty(e.REMITO))
            //    .ToListAsync();
            //}
            //else if(estado != EstadoPedido.PendienteRemitir || estado != EstadoPedido.Todos)
            //{
            //    return await _context.vEstadoPedidos.Where(e => e.ESTADO_PEDIDO == (int)estado)
            //    .ToListAsync();
            //}
            //            {
               // return await _context.vEstadoPedidos.ToListAsync();
            //}
            
            //else

        }

    }

}
