using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransportesController : ControllerBase
{
    private readonly vTransportesRepository _vTransportesRepository;
    private readonly int cg_cia_usuario = 1; /*CAMBIAR POR LA DEL USUARIO*/

    public TransportesController(vTransportesRepository vTransportesRepository)
    {
        _vTransportesRepository = vTransportesRepository;
    }

    // GET: api/Compras
    [HttpGet]
    public async Task<ActionResult<IEnumerable<vTransporte>>> GetCompras()
    {
        try
        {
            return await _vTransportesRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/Compras/5
    [HttpGet("{id}")]
    public async Task<ActionResult<vTransporte>> GetCompra(int id)
    {
        var cond = await _vTransportesRepository.ObtenerPorId(id);

        if (cond == null) return NotFound();

        return Ok(cond);
    }
}