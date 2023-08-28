using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;
using SupplyChain.Shared.Prod;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumenStockController : Controller
    {
        private readonly AppDbContext _context;

        public ResumenStockController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ResumenStocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<vResumenStock>>> GetResumenStock()
        {
            try
            {
                return await _context.vResumenStock.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/ResumenStocksPositivo/GetResumenStockPositivo
        [HttpGet("GetResumenStockPositivo")]
        public async Task<ActionResult<IEnumerable<vResumenStock>>> GetResumenStockPositivo(bool sinDepositoVentas = false)
        {
            try
            {
                var query = _context.vResumenStock.AsNoTracking()
                    .Where(rs => rs.STOCK > 0);

                if (sinDepositoVentas)
                {
                    query = query.Where(r => r.CG_DEP != 1);
                }

                var lista = await query.ToListAsync();
                return lista;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/ResumenStocksPositivo/ByCodigo/
        [HttpGet("ByCodigo")]
        public async Task<ActionResult<IEnumerable<vResumenStock>>> ByCodigo([FromQuery]FilterProd filter)
        {
            try
            {
                return await _context.vResumenStock
                    .Where(rs => rs.STOCK > 0 && rs.CG_ART.Trim() == filter.Codigo.Trim()).ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/ResumenStocksGetResumenStockByDeposito/1
        [HttpGet("GetResumenStockByDeposito/{cg_dep}")]
        public async Task<ActionResult<IEnumerable<vResumenStock>>> GetResumenStockByDeposito(int cg_dep)
        {
            return await _context.vResumenStock.Where(r=> r.CG_DEP == cg_dep).ToListAsync();
        }

        // GET: api/ResumenStocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<vResumenStock>> GetResumenStock(decimal id)
        {
            var resumenStock = await _context.vResumenStock.FindAsync(id);

            if (resumenStock == null)
            {
                return NotFound();
            }

            return resumenStock;
        }

        // GET: api/ResumenStocks/GetByStock
        [HttpGet("GetByStock")]
        public async Task<ActionResult<vResumenStock>> GetByStock([FromQuery] ResumenStock resumenStock)
        {
            try
            {
                
                resumenStock.DESPACHO = resumenStock.DESPACHO == null ? "" : resumenStock.DESPACHO ;
                resumenStock.LOTE = resumenStock.LOTE == null ? "" : resumenStock.LOTE ;
                resumenStock.SERIE = resumenStock.SERIE == null ? "" : resumenStock.SERIE ;



                var query = _context.vResumenStock.Where(r =>
                    r.CG_ART.ToUpper() == resumenStock.CG_ART.ToUpper()
                    && r.LOTE.ToUpper() == resumenStock.LOTE.ToUpper()
                    && r.DESPACHO.ToUpper() == resumenStock.DESPACHO.ToUpper()
                    && r.SERIE.ToUpper() == resumenStock.SERIE.ToUpper()
                ).AsQueryable();

                if (resumenStock.CG_DEP > 0)
                {
                   query = query.Where(r => r.CG_DEP == resumenStock.CG_DEP);
                }

                return await query.FirstOrDefaultAsync();
             }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }


        [HttpGet("GetStock")]
        public async Task<ActionResult<IEnumerable<vResumenStock>>> GetStock([FromQuery] vResumenStock resumenStock)
        {
            try
            {

                resumenStock.DESPACHO = resumenStock.DESPACHO == null ? "" : resumenStock.DESPACHO;
                resumenStock.LOTE = resumenStock.LOTE == null ? "" : resumenStock.LOTE;
                resumenStock.SERIE = resumenStock.SERIE == null ? "" : resumenStock.SERIE;
                return await _context.vResumenStock.Where(r =>
                    r.CG_DEP == resumenStock.CG_DEP
                    && r.CG_ART.ToUpper() == resumenStock.CG_ART.ToUpper()
                    && r.LOTE.ToUpper() == resumenStock.LOTE.ToUpper()
                    && r.DESPACHO.ToUpper() == resumenStock.DESPACHO.ToUpper()
                    && r.SERIE.ToUpper() == resumenStock.SERIE.ToUpper()
                ).ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }





        // PUT: api/ResumenStocks/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResumenStock(decimal id, ResumenStock resumenStock)
        {
            if (id != resumenStock.Registro)
            {
                return BadRequest();
            }

            _context.Entry(resumenStock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResumenStockExists(id))
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

        // POST: api/ResumenStocks
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ResumenStock>> PostResumenStock(ResumenStock resumenStock)
        {
            _context.ResumenStock.Add(resumenStock);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResumenStock", new { id = resumenStock.Registro }, resumenStock);
        }

        // DELETE: api/ResumenStocks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResumenStock>> DeleteResumenStock(decimal id)
        {
            var resumenStock = await _context.ResumenStock.FindAsync(id);
            if (resumenStock == null)
            {
                return NotFound();
            }

            _context.ResumenStock.Remove(resumenStock);
            await _context.SaveChangesAsync();

            return resumenStock;
        }

        private bool ResumenStockExists(decimal id)
        {
            return _context.ResumenStock.Any(e => e.Registro == id);
        }
    }
}
