using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneraController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GeneraController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Genera
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genera>>> GetGenera()
        {
            return await _context.Genera.ToListAsync();
        }

        // GET: api/Genera/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Genera>> GetGenera(string id)
        {
            var genera = await _context.Genera.FindAsync(id);

            if (genera == null)
            {
                return NotFound();
            }

            return genera;
        }

        // PUT: api/Genera/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenera(string id, Genera genera)
        {
            if (id != genera.CAMP3)
            {
                return BadRequest();
            }

            _context.Entry(genera).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeneraExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Genera
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Genera>> PostGenera(Genera genera)
        {
            _context.Genera.Add(genera);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GeneraExists(genera.CAMP3))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetGenera", new { id = genera.CAMP3 }, genera);
        }

        // DELETE: api/Genera/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenera(string id)
        {
            var genera = await _context.Genera.FindAsync(id);
            if (genera == null)
            {
                return NotFound();
            }

            _context.Genera.Remove(genera);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("Reserva/{campo}")]
        public async Task<ActionResult<Genera>> ReservaByCampo(string campo)
        {

            try
            {

                await Reserva(campo);
                var genera = await _context.Genera.Where(g => g.CAMP3 == campo).FirstOrDefaultAsync();
                return Ok(genera);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpGet("Libera/{campo}")]
        public async Task<ActionResult<Genera>> LiberaByCampo(string campo)
        {
            try
            {
                await Libera(campo);
                var genera = await _context.Genera.Where(g => g.CAMP3 == campo).FirstOrDefaultAsync();
                return Ok(genera);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        private bool GeneraExists(string id)
        {
            return _context.Genera.Any(e => e.CAMP3 == id);
        }

        private async Task Reserva(string campo)
        {
            var sp = $"Exec N_Genera 1, '{campo}', 'R', 0, '', 0, 0";
            await _context.Database.ExecuteSqlRawAsync(sp);
        }

        private async Task Libera(string campo)
        {
            var sp = $"Exec N_Genera 1, '{campo}', 'L', 0, '', 0, 0";
            await _context.Database.ExecuteSqlRawAsync(sp);
        }
    }
}
