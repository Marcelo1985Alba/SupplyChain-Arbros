using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class AspAmbController : ControllerBase
{
    private readonly AspAmbRepository _aspAmbRepository;

    public AspAmbController(AspAmbRepository aspAmbRepository)
    {
        _aspAmbRepository = aspAmbRepository;
    }

    // GET: api/AspAmb
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AspAmb>>> GetAspAmb()
    {
        try
        {
            return await _aspAmbRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/AspAmb/Existe/{id}
    [HttpGet("Existe/{id}")]
    public async Task<ActionResult<bool>> ExisteAspAmb(int id)
    {
        try
        {
            return await _aspAmbRepository.Existe(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // PUT: api/AspAmb/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAspAmb(int id, AspAmb aspAmb)
    {
        if (id != aspAmb.Id) return BadRequest();

        try
        {
            await _aspAmbRepository.Actualizar(aspAmb);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _aspAmbRepository.Existe(id))
                return NotFound();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(aspAmb);
    }

    // POST: api/AspAmb
    [HttpPost]
    public async Task<ActionResult<AspAmb>> PostAspAmb(AspAmb aspAmb)
    {
        try
        {
            await _aspAmbRepository.Agregar(aspAmb);
            return CreatedAtAction("GetAspAmb", new { id = aspAmb.Id }, aspAmb);
        }
        catch (DbUpdateException exx)
        {
            if (!await _aspAmbRepository.Existe(aspAmb.Id))
                return Conflict();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // DELETE: api/AspAmb/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<AspAmb>> DeleteAspAmb(int id)
    {
        var aspAmb = await _aspAmbRepository.ObtenerPorId(id);
        if (aspAmb == null) return NotFound();

        await _aspAmbRepository.Remover(id);

        return aspAmb;
    }

    // POST: api/AspAmb/PostList
    [HttpPost("PostList")]
    public async Task<ActionResult<AspAmb>> PostList(List<AspAmb> aspAmb)
    {
        try
        {
            foreach (var item in aspAmb) await _aspAmbRepository.Remover(item.Id);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }

        return Ok();
    }
}