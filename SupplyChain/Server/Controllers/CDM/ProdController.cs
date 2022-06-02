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


        [HttpGet("GetProdAndReparaciones")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProdAndReparaciones()
        {
            try
            {
                return await _productoRepository
                    .Obtener(p=> p.CG_ORDEN == 1 || p.CG_ORDEN == 13).ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        // GET: api/Prod/5
        [HttpGet("ExisteProducto/{id}")]
        public async Task<ActionResult<bool>> ExisteProducto(string id)
        {
            var existe = await _productoRepository.Existe(id);

            return existe;
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
            if (id != Prod.Id)
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
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok(Prod);
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
                return CreatedAtAction("GetProd", new { id = Prod.Id }, Prod);
            }
            catch (DbUpdateException exx) 
            {
                if (!await _productoRepository.Existe(Prod.Id))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("PostList")]
        public async Task<ActionResult<Producto>> PostList(List<Producto> productos)
        {
            try
            {
                foreach (var item in productos)
                {
                    await _productoRepository.Remover(item.Id.Trim());
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok();
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
            if (await _productoRepository.Existe(CG_PROD))
            {
                lDesProd = await _productoRepository.Obtener(p => p.Id == CG_PROD).ToListAsync();
            }
            return lDesProd == null ? NotFound() : lDesProd;
        }

        // GET: api/Prod/BuscarPorDES_PROD/{DES_PROD}
        [HttpGet("BuscarPorDES_PROD/{DES_PROD}")]
        public async Task<ActionResult<List<Producto>>> BuscarPorDES_PROD(string DES_PROD)
        {
            List<Producto> lDesProd = new List<Producto>();
            lDesProd = await _productoRepository.Obtener(p => p.DES_PROD == DES_PROD).ToListAsync();
            if (lDesProd == null)
            {
                return NotFound();
            }
            return lDesProd;
        }
    }
}