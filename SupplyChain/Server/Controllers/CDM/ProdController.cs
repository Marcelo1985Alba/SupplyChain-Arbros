using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared.Models;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Prod
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProd()
        {
            try
            {
                return await _context.Prod.ToListAsync();
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }

            
        }


        [HttpGet("GetPedidos")]
        public IEnumerable<Producto> Gets(string PEDIDO)
        {
            string xSQL = string.Format("SELECT * FROM Prod ");
            return _context.Prod.FromSqlRaw(xSQL).ToList();
        }


        // GET: api/Prod/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProd(string id)
        {
            var Prod = await _context.Prod.Where(P=> P.CG_PROD == id).FirstOrDefaultAsync();

            if (Prod == null)
            {
                return NotFound();
            }

            return Prod;
        }

        // PUT: api/Prod/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProd(string id, Prod Prod)
        {
            if (id != Prod.CG_PROD)
            {
                return BadRequest();
            }

            _context.Entry(Prod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdExists(id))
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
        public async Task<ActionResult<Prod>> PostProd(Producto Prod)
        {
            _context.Prod.Add(Prod);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProdExists(Prod.CG_PROD))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProd", new { id = Prod.CG_PROD }, Prod);
        }

        // DELETE: api/Prod/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Producto>> DeleteProd(string id)
        {
            var Prod = await _context.Prod.FindAsync(id);
            if (Prod == null)
            {
                return NotFound();
            }

            _context.Prod.Remove(Prod);
            await _context.SaveChangesAsync();

            return Prod;
        }

        private bool ProdExists(string id)
        {
            return _context.Prod.Any(e => e.CG_PROD == id);
        }

        // GET: api/Prod/BuscarPorCG_PROD/{CG_PROD}
        [HttpGet("BuscarPorCG_PROD/{CG_PROD}")]
        public async Task<ActionResult<List<Producto>>> BuscarPorCG_PROD(string CG_PROD)
        {
            List<Producto> lDesProd = new List<Producto>();
            if (_context.Prod.Any())
            {
                lDesProd = await _context.Prod.Where(p => p.CG_PROD == CG_PROD).ToListAsync();
            }
            if (lDesProd == null)
            {
                return NotFound();
            }
            return lDesProd;
        }
    }
}