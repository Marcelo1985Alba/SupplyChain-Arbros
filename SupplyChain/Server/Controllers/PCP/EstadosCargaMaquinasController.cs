using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain;
using SupplyChain.Shared.PCP;

namespace SupplyChain.Server.Controllers.PCP
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadosCargaMaquinasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstadosCargaMaquinasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EstadosCargaMaquinas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstadosCargaMaquina>>> GetEstadosCargaMaquinas()
        {
            return await _context.EstadosCargaMaquinas.ToListAsync();
        }

        // GET: api/EstadosCargaMaquinas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EstadosCargaMaquina>> GetEstadosCargaMaquina(int id)
        {
            var estadosCargaMaquina = await _context.EstadosCargaMaquinas.FindAsync(id);

            if (estadosCargaMaquina == null)
            {
                return NotFound();
            }

            return estadosCargaMaquina;
        }

        // PUT: api/EstadosCargaMaquinas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstadosCargaMaquina(int id, EstadosCargaMaquina estadosCargaMaquina)
        {
            if (id != estadosCargaMaquina.CG_ESTADO)
            {
                return BadRequest();
            }

            _context.Entry(estadosCargaMaquina).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstadosCargaMaquinaExists(id))
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

        // POST: api/EstadosCargaMaquinas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EstadosCargaMaquina>> PostEstadosCargaMaquina(EstadosCargaMaquina estadosCargaMaquina)
        {
            _context.EstadosCargaMaquinas.Add(estadosCargaMaquina);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEstadosCargaMaquina", new { id = estadosCargaMaquina.CG_ESTADO }, estadosCargaMaquina);
        }

        // DELETE: api/EstadosCargaMaquinas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstadosCargaMaquina(int id)
        {
            var estadosCargaMaquina = await _context.EstadosCargaMaquinas.FindAsync(id);
            if (estadosCargaMaquina == null)
            {
                return NotFound();
            }

            _context.EstadosCargaMaquinas.Remove(estadosCargaMaquina);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EstadosCargaMaquinaExists(int id)
        {
            return _context.EstadosCargaMaquinas.Any(e => e.CG_ESTADO == id);
        }
    }
}
