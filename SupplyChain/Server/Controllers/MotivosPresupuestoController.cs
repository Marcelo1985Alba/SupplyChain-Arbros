using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MotivosPresupuestoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly MotivosPresupuestoRepository _motivosPresupuestoRepository;

    public MotivosPresupuestoController(MotivosPresupuestoRepository motivosPresupuestoRepository)
    {
        _motivosPresupuestoRepository = motivosPresupuestoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MotivosPresupuesto>>> GetMotivosTodos()
    {
        try
        {
            var result = await _motivosPresupuestoRepository.ObtenerTodos();
            return result;
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("{motivo}")]
    public async Task<MotivosPresupuesto> GetMotivos(string motivo)
    {
        var motivos = await _motivosPresupuestoRepository.Obtener(m => m.Motivo == motivo).FirstOrDefaultAsync();

        return motivos;
    }
}