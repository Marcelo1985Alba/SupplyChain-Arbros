using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TiposCambioController : ControllerBase
{
    private readonly vTipoCambioRepository _vTipoCambioRepository;
    private readonly int cg_cia_usuario = 1; /*CAMBIAR POR LA DEL USUARIO*/

    public TiposCambioController(vTipoCambioRepository vTipoCambioRepository)
    {
        _vTipoCambioRepository = vTipoCambioRepository;
    }

    // GET: api/Compras
    [HttpGet]
    public async Task<ActionResult<IEnumerable<vTipoCambio>>> GetCompras()
    {
        try
        {
            return await _vTipoCambioRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/Compras/5
    [HttpGet("{id}")]
    public async Task<ActionResult<vTipoCambio>> Get(int id)
    {
        var tc = await _vTipoCambioRepository.ObtenerPorId(id);

        if (tc == null) return NotFound();

        return Ok(tc);
    }

    [HttpGet("GetUltimaCotizacionDolar")]
    public async Task<ActionResult<decimal>> GetUltimaCotizacionDolar()
    {
        var tc = await _vTipoCambioRepository.Obtener(t => t.Id_Moneda == 2)
            .OrderByDescending(o => o.Fecha_Cotiz)
            .FirstOrDefaultAsync();

        if (tc == null) return NotFound();

        return Ok(tc.Cotizacion);
    }
}