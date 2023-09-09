using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CondicionesEntregaController : ControllerBase
{
    private readonly vCondicionesEntregaRepository _vCondicionesEntregaRepository;
    private readonly int cg_cia_usuario = 1; /*CAMBIAR POR LA DEL USUARIO*/

    public CondicionesEntregaController(vCondicionesEntregaRepository vCondicionesEntregaRepository)
    {
        _vCondicionesEntregaRepository = vCondicionesEntregaRepository;
    }

    // GET: api/Compras
    [HttpGet]
    public async Task<ActionResult<IEnumerable<vCondicionesEntrega>>> GetCompras()
    {
        try
        {
            return await _vCondicionesEntregaRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/Compras/5
    [HttpGet("{id}")]
    public async Task<ActionResult<vCondicionesEntrega>> GetCompra(int id)
    {
        var cond = await _vCondicionesEntregaRepository.ObtenerPorId(id);

        if (cond == null) return NotFound();

        return Ok(cond);
    }
}