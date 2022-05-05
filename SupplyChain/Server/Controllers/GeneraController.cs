using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneraController : ControllerBase
    {
        private readonly GeneraRepository _generaRepository;

        public GeneraController(GeneraRepository generaRepository)
        {
            _generaRepository = generaRepository;
        }

        // GET: api/Genera
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genera>>> GetGenera()
        {
            return await _generaRepository.ObtenerTodos();
        }

        // GET: api/Genera/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Genera>> GetGenera(string id)
        {
            var genera = await _generaRepository.ObtenerPorId(id);

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
            if (id != genera.Id)
            {
                return BadRequest();
            }

            try
            {
                await _generaRepository.Actualizar(genera);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await _generaRepository.Existe(id))
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
            try
            {
                await _generaRepository.Agregar(genera);
            }
            catch (DbUpdateException)
            {
                if (await _generaRepository.Existe(genera.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetGenera", new { id = genera.Id }, genera);
        }

        // DELETE: api/Genera/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenera(string id)
        {
            var genera = await _generaRepository.ObtenerPorId(id);
            if (genera == null)
            {
                return NotFound();
            }

            await _generaRepository.Remover(id);

            return NoContent();
        }

        [HttpGet("Reserva/{campo}")]
        public async Task<ActionResult<Genera>> ReservaByCampo(string campo)
        {

            try
            {

                await _generaRepository.Reserva(campo);
                var genera = await _generaRepository.Obtener(g => g.Id == campo).FirstOrDefaultAsync();
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
                await _generaRepository.Libera(campo);
                var genera = await _generaRepository.Obtener(g => g.Id == campo).FirstOrDefaultAsync();
                return Ok(genera);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }
    }
}
