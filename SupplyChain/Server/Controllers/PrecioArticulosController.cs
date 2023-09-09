using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrecioArticulosController : ControllerBase
{
    private readonly PrecioArticulosRepository _precioArticulosRepository;

    public PrecioArticulosController(PrecioArticulosRepository precioArticulosRepository)
    {
        _precioArticulosRepository = precioArticulosRepository;
    }

    // GET: api/PrecioArticulos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PreciosArticulos>>> Gets()
    {
        try
        {
            return await _precioArticulosRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/PrecioArticulos/Reparaciones
    [HttpGet("Reparaciones")]
    public async Task<ActionResult<IEnumerable<PreciosArticulos>>> Reparaciones()
    {
        try
        {
            return await _precioArticulosRepository.Obtener(p => p.Id.StartsWith("00")).ToListAsync();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/PrecioArticulos/Search/codigo/descripcion
    [HttpGet("Search/{codigo}/{descripcion}")]
    public async Task<ActionResult<IEnumerable<PreciosArticulos>>> Search(string codigo, string descripcion)
    {
        try
        {
            return Ok(await _precioArticulosRepository.Search(codigo, descripcion));
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/PrecioArticulos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Solicitud>> Get(string id)
    {
        var precioArt = await _precioArticulosRepository.ObtenerPorId(id);

        if (precioArt == null) return NotFound();

        return Ok(precioArt);
    }

    // GET: api/PrecioArticulos/id
    [HttpGet("Existe/{id}")]
    public async Task<ActionResult<bool>> Existe(string id)
    {
        var existe = await _precioArticulosRepository.Existe(id);

        return Ok(existe);
    }

    // PUT: api/PrecioArticulos/id
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, PreciosArticulos precioArt)
    {
        if (id != precioArt.Id) return BadRequest();

        try
        {
            await _precioArticulosRepository.Actualizar(precioArt);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _precioArticulosRepository.Existe(id))
                return NotFound();
            return BadRequest();
        }

        return Ok(precioArt);
    }

    // POST: api/PrecioArticulos
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPost]
    public async Task<ActionResult<PreciosArticulos>> Post(PreciosArticulos precioArt)
    {
        try
        {
            await _precioArticulosRepository.Agregar(precioArt);
            return CreatedAtAction("Get", new { id = precioArt.Id }, precioArt);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // DELETE: api/PrecioArticulos/id
    [HttpDelete("{id}")]
    public async Task<ActionResult<PreciosArticulos>> DeleteCompra(string id)
    {
        var solicitud = await _precioArticulosRepository.ObtenerPorId(id);
        if (solicitud == null) return NotFound();

        await _precioArticulosRepository.Remover(id);

        return solicitud;
    }

    // GET: api/PrecioArticulos/GetPrecio/id
    [HttpGet("GetPrecio/{id}")]
    public async Task<decimal> GetPrecio(string id)
    {
        decimal? precio = 0;
        if (await _precioArticulosRepository.Existe(id))
            precio = _precioArticulosRepository.Obtener(p => p.Id == id).FirstOrDefaultAsync().Result.Precio;
        return (decimal)precio;
    }
}