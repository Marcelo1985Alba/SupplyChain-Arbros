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
    public class ProdController : ControllerBase
    {
        private readonly ProductoRepository _productoRepository;

        public ProdController(ProductoRepository productoRepository)
        {
            this._productoRepository = productoRepository;
        }

        // GET: api/Prod
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProd()
        {
            try
            {
                return await _productoRepository.ObtenerTodos();
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }



        // GET: api/Prod/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> Get(string id)
        {
            var Prod = await _productoRepository.ObtenerPorId(id);

            return Prod ?? (ActionResult<Producto>)NotFound();
        }

        // GET: api/Prod/5
        [HttpGet("GetByFilter")]
        public async Task<ActionResult<Producto>> GetByFilter([FromQuery]FilterProd filter)
        {
            var Prod = await _productoRepository.ObtenerPorId(filter.Codigo);

            return Prod ?? (ActionResult<Producto>)NotFound();
        }


        // PUT: api/Prod/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProd(string id, Producto Prod)
        {
            if (id != Prod.CG_PROD)
            {
                return BadRequest();
            }

            try
            {
                await _productoRepository.Actualizar(Prod);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _productoRepository.Existe(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }

            return NoContent();
        }

        // POST: api/Prod
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProd(Producto Prod)
        {
            try
            {
                await _productoRepository.Agregar(Prod);
            }
            catch (DbUpdateException) 
            {
                if (!await _productoRepository.Existe(Prod.CG_PROD))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest();
                }
            }

            return CreatedAtAction("GetProd", new { id = Prod.CG_PROD }, Prod);
        }

        // DELETE: api/Prod/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Producto>> DeleteProd(string id)
        {
            var Prod = await _productoRepository.ObtenerPorId(id);
            if (Prod == null)
            {
                return NotFound();
            }

            await _productoRepository.Remover(id);

            return Prod;
        }


        // GET: api/Prod/BuscarPorCG_PROD/{CG_PROD}
        [HttpGet("BuscarPorCG_PROD/{CG_PROD}")]
        public async Task<ActionResult<List<Producto>>> BuscarPorCG_PROD(string CG_PROD)
        {
            List<Producto> lDesProd = new List<Producto>();
            if (!await _productoRepository.Existe(CG_PROD))
            {
                lDesProd = (List<Producto>)await _productoRepository.Obtener(p => p.CG_PROD == CG_PROD).ToListAsync();
            }
            return lDesProd == null ? NotFound() : lDesProd;
        }
    }
}