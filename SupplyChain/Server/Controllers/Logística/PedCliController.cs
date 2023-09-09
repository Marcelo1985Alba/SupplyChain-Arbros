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
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Logística;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PedCliController : ControllerBase
{
    private readonly PedCliRepository _pedCliRepository;
    private readonly UserManager<ApplicationUser> userManager;

    public PedCliController(PedCliRepository pedCliRpository, UserManager<ApplicationUser> userManager)
    {
        _pedCliRepository = pedCliRpository;
        this.userManager = userManager;
    }

    [HttpGet]
    public async Task<IEnumerable<PedCli>> Gets()
    {
        return await _pedCliRepository.ObtenerTodos();
    }

    [HttpGet("ByFilter/{tipoFiltro}")]
    public async Task<IEnumerable<PedCli>> GetByFilter(TipoFiltro tipoFiltro = TipoFiltro.Todos)
    {
        return await _pedCliRepository.ByFilter(tipoFiltro);
    }

    [HttpGet("ObtenerPedCliPedidos")]
    public async Task<IEnumerable<PedCli>> Get()
    {
        return await _pedCliRepository.ObtenerPedCliPedidos();
    }

    [HttpGet("{id}")]
    public async Task<PedCli> Get(int id)
    {
        return await _pedCliRepository.Obtener(p => p.Id == id).FirstOrDefaultAsync();
    }


    [HttpGet("ByPedido/{pedido}")]
    public async Task<IEnumerable<PedCli>> GetByPedido(int pedido)
    {
        var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();
        if (roleClaims.Any(c => c.Value == "Cliente"))
        {
            var userName = HttpContext.User.Identity.Name;
            var user = await userManager.FindByNameAsync(userName);
            var cg_cli_usuario = user.Cg_Cli;
            return await _pedCliRepository.Obtener(p => p.CG_CLI == cg_cli_usuario && p.PEDIDO == pedido)
                .ToListAsync();
        }


        return await _pedCliRepository.Obtener(p => p.PEDIDO == pedido).ToListAsync();
    }

    [HttpGet("ByNumOci/{numOci}")]
    public async Task<IEnumerable<PedCli>> ByNumOci(int numOci)
    {
        return await _pedCliRepository.Obtener(p => p.NUMOCI == numOci).ToListAsync();
    }

    // PUT: api/Servicios/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPedCli(int id, PedCli Pedclis)
    {
        if (id != Pedclis.PEDIDO) return BadRequest();


        try
        {
            await _pedCliRepository.Actualizar(Pedclis);
        }
        catch (Exception ex)
        {
            if (!await _pedCliRepository.Existe(id))
                return NotFound(ex);
            return BadRequest(ex);
        }

        return NoContent();
    }


    // GET: api/pedcli/BuscarPorPedido/{PEDIDO}
    [HttpGet("BuscarPorPedido/{PEDIDO}")]
    public async Task<ActionResult<List<PedCli>>> BuscarPorPedido(string PEDIDO)
    {
        var lpedcli = await _pedCliRepository.Obtener(p => p.PEDIDO.ToString() == PEDIDO).ToListAsync();
        return lpedcli == null ? NotFound() : lpedcli;
    }

    [HttpGet("GetPedidoEncabezadoById/{id}")]
    public async Task<PedCliEncabezado> GetPedidoEncabezadoById(int id)
    {
        return await _pedCliRepository.ObtenerPedidosEncabezado(id);
    }

    [HttpGet("GetPedidoEncabezadoByNumOci/{numOci}")]
    public async Task<PedCliEncabezado> GetPedidoEncabezadoByNumOci(int numOci)
    {
        return await _pedCliRepository.ObtenerPedidosEncabezadoByNumOci(numOci);
    }

    [HttpPost("PostList")]
    public async Task<ActionResult<List<PedCli>>> PostList(List<PedCli> lista)
    {
        try
        {
            await _pedCliRepository.GuardarList(lista);
            return lista;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE api/<PresupuestosController>/5
    [HttpDelete("{pedido}")]
    public async Task<ActionResult<PedCli>> DeletePedido(int pedido)
    {
        try
        {
            var pedcli = await _pedCliRepository.Obtener(p => p.PEDIDO == pedido)
                .FirstOrDefaultAsync();
            if (pedcli == null) return NotFound();

            if (await _pedCliRepository.TieneRemito(pedido)) return Conflict("El pedido tiene remito asociado.");


            await _pedCliRepository.Remover(pedcli.Id);


            return pedcli;
        }
        catch (Exception ex)
        {
            return BadRequest("Error al eliminar Presupuesto " + ex.Message);
        }
    }


    [HttpDelete("PorOci/{oci}")]
    public async Task<ActionResult<PedCli>> DeleteOCI(int oci)
    {
        try
        {
            var listaPedcli = await _pedCliRepository.Obtener(p => p.NUMOCI == oci)
                .ToListAsync();
            if (listaPedcli == null || listaPedcli.Count == 0) return NotFound();

            foreach (var item in listaPedcli)
                if (await _pedCliRepository.TieneRemito(item.PEDIDO))
                    return Conflict($"El pedido {item.PEDIDO} tiene remito asociado.");


            await _pedCliRepository.RemoverList(listaPedcli.Select(p => p.Id).ToList());


            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest("Error al eliminar Presupuesto " + ex.Message);
        }
    }
}