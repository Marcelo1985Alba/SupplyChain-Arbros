using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.Prod;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreciosArtController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PreciosArtController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Prod
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreciosArticulos>>> GetPreciosArt()
        {
            return await _context.PrecioArticulo.ToListAsync();
        }


        // GET: api/Prod/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PreciosArticulos>> GetPrecioArt(string id)
        {
            var precio = await _context.PrecioArticulo.FindAsync(id);

            if (precio == null)
            {
                return NotFound();
            }
            return precio;
        }

        // PUT: api/Prod/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPreciosArt(string id, PreciosArticulos precio)
        {
            if (id != precio.Id)
            {
                return BadRequest();
            }

            _context.Entry(precio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PreciosArtExists(id))
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

        // POST: api/Prod
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PreciosArticulos>> PostPreciosArt(PreciosArticulos precio)
        {
            _context.PrecioArticulo.Add(precio);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PreciosArtExists(precio.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPrecioArt", new { id = precio.Id }, precio);
        }
        private bool PreciosArtExists(string id)
        {
            return _context.PrecioArticulo.Any(e => e.Id == id);
        }
    }
}