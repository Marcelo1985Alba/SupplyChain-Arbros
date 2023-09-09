using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class DepositoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly int cg_cia_usuario = 1; /*CAMBIAR POR LA DEL USUARIO QUE INGRESÓ*/

    public DepositoController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Depositos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Deposito>>> GetDeposito()
    {
        try
        {
            return await _context.Depositos.Where(d => d.CG_CIA == cg_cia_usuario).ToListAsync();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("GetDepositos")]
    public object GetDepositos()
    {
        var data = _context.Depositos.AsQueryable();
        var count = data.Count();
        var queryString = Request.Query;
        if (queryString.Keys.Contains("$inlinecount"))
        {
            StringValues Skip;
            StringValues Take;
            var skip = queryString.TryGetValue("$skip", out Skip) ? Convert.ToInt32(Skip[0]) : 0;
            var top = queryString.TryGetValue("$top", out Take) ? Convert.ToInt32(Take[0]) : data.Count();
            return new { Items = data.Skip(skip).Take(top), Count = count };
        }

        return data;
    }

    // GET: api/Depositos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Deposito>> GetDeposito(int id)
    {
        var deposito = await _context.Depositos.FindAsync(id);

        if (deposito == null) return NotFound();

        return deposito;
    }

    // PUT: api/Depositos/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDeposito(int id, Deposito deposito)
    {
        if (id != deposito.CG_DEP) return BadRequest();

        _context.Entry(deposito).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DepositoExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/Depositos
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPost]
    public async Task<ActionResult<Deposito>> PostDeposito(Deposito deposito)
    {
        _context.Depositos.Add(deposito);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetDeposito", new { id = deposito.CG_DEP }, deposito);
    }

    // DELETE: api/Depositos/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Deposito>> DeleteDeposito(int id)
    {
        var deposito = await _context.Depositos.FindAsync(id);
        if (deposito == null) return NotFound();

        _context.Depositos.Remove(deposito);
        await _context.SaveChangesAsync();

        return deposito;
    }

    private bool DepositoExists(int id)
    {
        return _context.Depositos.Any(e => e.CG_DEP == id);
    }
}