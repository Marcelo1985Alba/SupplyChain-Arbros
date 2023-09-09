using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class UnidadesController : ControllerBase
{
    private readonly UnidadesRepository _unidadesRepository;

    public UnidadesController(UnidadesRepository unidadesRepository)
    {
        _unidadesRepository = unidadesRepository;
    }

    // GET: api/Unidades
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Unidades>>> GetUnidades()
    {
        try
        {
            return await _unidadesRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/Unidades/Existe/{id}
    [HttpGet("Existe/{id}")]
    public async Task<ActionResult<bool>> ExisteUnidad(string id)
    {
        try
        {
            return await _unidadesRepository.Existe(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // PUT: api/Unidades/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUnidades(string id, Unidades Unidad)
    {
        if (id != Unidad.Id) return BadRequest();

        try
        {
            await _unidadesRepository.Actualizar(Unidad);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _unidadesRepository.Existe(id))
                return NotFound();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(Unidad);
    }

    // POST: api/Unidades
    [HttpPost]
    public async Task<ActionResult<Unidades>> PostUnidades(Unidades Unidad)
    {
        try
        {
            await _unidadesRepository.Agregar(Unidad);
            return CreatedAtAction("GetUnidades", new { id = Unidad.Id }, Unidad);
        }
        catch (DbUpdateException exx)
        {
            if (!await _unidadesRepository.Existe(Unidad.Id))
                return Conflict();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // DELETE: api/Unidades/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<Unidades>> DeleteUnidades(string id)
    {
        var Unidad = await _unidadesRepository.ObtenerPorId(id);
        if (Unidad == null) return NotFound();

        await _unidadesRepository.Remover(id);

        return Unidad;
    }

    // POST: api/Unidades/PostList
    [HttpPost("PostList")]
    public async Task<ActionResult<Unidades>> PostList(List<Unidades> unidades)
    {
        try
        {
            foreach (var item in unidades) await _unidadesRepository.Remover(item.Id);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }

        return Ok();
    }
}