using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class ISOController : ControllerBase
{
    private readonly ISORepository _isoRepository;

    public ISOController(ISORepository isoRepository)
    {
        _isoRepository = isoRepository;
    }

    // GET: api/ISO
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ISO>>> GetISO()
    {
        try
        {
            return await _isoRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/ISO/Existe/{id}
    [HttpGet("Existe/{id}")]
    public async Task<ActionResult<bool>> ExisteISO(int id)
    {
        try
        {
            return await _isoRepository.Existe(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // PUT: api/ISO/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutISO(int id, ISO iso)
    {
        if (id != iso.Id) return BadRequest();

        try
        {
            await _isoRepository.Actualizar(iso);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _isoRepository.Existe(id))
                return NotFound();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(iso);
    }

    // POST: api/ISO
    [HttpPost]
    public async Task<ActionResult<ISO>> PostISO(ISO iso)
    {
        try
        {
            await _isoRepository.Agregar(iso);
            return CreatedAtAction("GetISO", new { id = iso.Id }, iso);
        }
        catch (DbUpdateException exx)
        {
            if (!await _isoRepository.Existe(iso.Id))
                return Conflict();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // DELETE: api/ISO/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<ISO>> DeleteISO(int id)
    {
        var iso = await _isoRepository.ObtenerPorId(id);
        if (iso == null) return NotFound();

        await _isoRepository.Remover(id);

        return iso;
    }

    // POST: api/ISO/PostList
    [HttpPost("PostList")]
    public async Task<ActionResult<ISO>> PostList(List<ISO> iso)
    {
        try
        {
            foreach (var item in iso) await _isoRepository.Remover(item.Id);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }

        return Ok();
    }
}