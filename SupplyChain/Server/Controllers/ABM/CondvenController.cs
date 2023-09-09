using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class CondvenController : ControllerBase
{
    private readonly AppDbContext _context;

    public CondvenController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Condven
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Condven>>> GetCondven()
    {
        return await _context.Condven.ToListAsync();
    }

    // GET: api/Condven
    [HttpGet("Itris")]
    public async Task<ActionResult<IEnumerable<vCondicionesPago>>> GetCondvenItris()
    {
        return await _context.vCondicionesPago.ToListAsync();
    }

    // GET: api/Condven/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Condven>> GetCondven(string id)
    {
        var Condven = await _context.Condven.FindAsync(id);

        if (Condven == null) return NotFound();

        return Condven;
    }

    // PUT: api/Condven/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCondven(string id, Condven Condven)
    {
        if (id != Condven.CG_CONDV) return BadRequest();

        _context.Entry(Condven).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CondvenExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/Condven
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPost]
    public async Task<ActionResult<Condven>> PostCondven(Condven Condven)
    {
        _context.Condven.Add(Condven);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (CondvenExists(Condven.CG_CONDV))
                return Conflict();
            throw;
        }

        return CreatedAtAction("GetCondven", new { id = Condven.CG_CONDV }, Condven);
    }

    // DELETE: api/Condven/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Condven>> DeleteCondven(string id)
    {
        var Condven = await _context.Condven.FindAsync(id);
        if (Condven == null) return NotFound();

        _context.Condven.Remove(Condven);
        await _context.SaveChangesAsync();

        return Condven;
    }

    private bool CondvenExists(string id)
    {
        return _context.Condven.Any(e => e.CG_CONDV == id);
    }
}