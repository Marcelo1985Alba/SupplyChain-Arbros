using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PresupuestosAnteriorController : ControllerBase
{
    private readonly GeneraRepository _generaRepository;


    private readonly PresupuestoAnteriorRepository _presupuestoRepository;

    public PresupuestosAnteriorController(PresupuestoAnteriorRepository presupuestoRepository,
        GeneraRepository generaRepository)
    {
        _presupuestoRepository = presupuestoRepository;
        _generaRepository = generaRepository;
    }

    // GET: api/<PresupuestosController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "value1", "value2" };
    }

    // GET api/<PresupuestosController>/5
    [HttpGet("{id}")]
    public async Task<PresupuestoAnterior> GetPresupuesto(int id)
    {
        return await _presupuestoRepository.Obtener(p => p.PRESUP == id).FirstOrDefaultAsync();
    }

    // POST api/<PresupuestosController>
    [HttpPost]
    public async Task<ActionResult<PresupuestoAnterior>> Post(PresupuestoAnterior presupuesto)
    {
        return await AgregarNuevoPresupuesto(presupuesto);
    }

    private async Task<ActionResult<PresupuestoAnterior>> AgregarNuevoPresupuesto(PresupuestoAnterior presupuesto)
    {
        await _presupuestoRepository.Agregar(presupuesto);

        return CreatedAtAction("GetPresupuesto", new { id = presupuesto.PRESUP }, presupuesto);
    }

    [HttpPost("PostFromSolicitud")]
    public async Task<ActionResult<PresupuestoAnterior>> PostFromSolicitud(PresupuestoAnterior presupuesto)
    {
        try
        {
            await _presupuestoRepository.AgregarDatosFaltantes(presupuesto);

            return await AgregarNuevoPresupuesto(presupuesto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // PUT api/<PresupuestosController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<PresupuestosController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}