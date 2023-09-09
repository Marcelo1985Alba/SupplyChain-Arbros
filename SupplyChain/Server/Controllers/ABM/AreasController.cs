using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class AreasController : ControllerBase
{
    private readonly AreasRepository _areasRepository;

    public AreasController(AreasRepository areasRepository)
    {
        _areasRepository = areasRepository;
    }

    // GET: api/Areas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Areas>>> GetAreas()
    {
        try
        {
            return await _areasRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/Areas/Existe/{id}
    [HttpGet("Existe/{id}")]
    public async Task<ActionResult<bool>> ExisteArea(int id)
    {
        try
        {
            return await _areasRepository.Existe(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // PUT: api/Areas/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAreas(int id, Areas Area)
    {
        if (id != Area.Id) return BadRequest();

        try
        {
            await _areasRepository.Actualizar(Area);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _areasRepository.Existe(id))
                return NotFound();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(Area);
    }

    // POST: api/Areas
    [HttpPost]
    public async Task<ActionResult<Areas>> PostAreas(Areas Area)
    {
        try
        {
            await _areasRepository.Agregar(Area);
            return CreatedAtAction("GetAreas", new { id = Area.Id }, Area);
        }
        catch (DbUpdateException exx)
        {
            if (!await _areasRepository.Existe(Area.Id))
                return Conflict();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // DELETE: api/Areas/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<Areas>> DeleteAreas(int id)
    {
        var Area = await _areasRepository.ObtenerPorId(id);
        if (Area == null) return NotFound();

        await _areasRepository.Remover(id);

        return Area;
    }

    // POST: api/Areas/PostList
    [HttpPost("PostList")]
    public async Task<ActionResult<Areas>> PostList(List<Areas> areas)
    {
        try
        {
            foreach (var item in areas) await _areasRepository.Remover(item.Id);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }

        return Ok();
    }
}