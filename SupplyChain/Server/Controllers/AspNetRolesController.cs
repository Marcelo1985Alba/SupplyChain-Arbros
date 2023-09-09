using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class AspNetRolesController : ControllerBase
{
    private readonly AspNetRolesRepository _aspNetRolesRepository;

    public AspNetRolesController(AspNetRolesRepository aspNetRolesRepository)
    {
        _aspNetRolesRepository = aspNetRolesRepository;
    }

    // GET: api/AspNetRoles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AspNetRoles>>> GetAspNetRoles()
    {
        try
        {
            return await _aspNetRolesRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/AspNetRoles/Existe/{id}
    [HttpGet("Existe/{id}")]
    public async Task<ActionResult<bool>> ExisteAspNetRoles(string id)
    {
        try
        {
            return await _aspNetRolesRepository.Existe(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // PUT: api/AspNetRoles/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAspNetRoles(string id, AspNetRoles aspNetRoles)
    {
        if (id != aspNetRoles.Id) return BadRequest();

        try
        {
            await _aspNetRolesRepository.Actualizar(aspNetRoles);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _aspNetRolesRepository.Existe(id))
                return NotFound();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok(aspNetRoles);
    }

    // POST: api/AspNetRoles
    [HttpPost]
    public async Task<ActionResult<AspNetRoles>> PostAspNetRoles(AspNetRoles aspNetRoles)
    {
        try
        {
            await _aspNetRolesRepository.Agregar(aspNetRoles);
            return CreatedAtAction("GetAspNetRoles", new { id = aspNetRoles.Id }, aspNetRoles);
        }
        catch (DbUpdateException exx)
        {
            if (!await _aspNetRolesRepository.Existe(aspNetRoles.Id))
                return Conflict();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // DELETE: api/AspNetRoles/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<AspNetRoles>> DeleteAspNetRoles(string id)
    {
        var aspNetRoles = await _aspNetRolesRepository.ObtenerPorId(id);
        if (aspNetRoles == null) return NotFound();

        await _aspNetRolesRepository.Remover(id);

        return aspNetRoles;
    }

    // POST: api/AspNetRoles/PostList
    [HttpPost("PostList")]
    public async Task<ActionResult<AspNetRoles>> PostList(List<AspNetRoles> aspNetRoles)
    {
        try
        {
            foreach (var item in aspNetRoles) await _aspNetRolesRepository.Remover(item.Id);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }

        return Ok();
    }
}