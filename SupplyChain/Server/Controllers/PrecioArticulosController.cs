using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrecioArticulosController : ControllerBase
    {
        private readonly PrecioArticulosRepository _precioArticulosRepository;

        public PrecioArticulosController(PrecioArticulosRepository precioArticulosRepository)
        {
            _precioArticulosRepository = precioArticulosRepository;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreciosArticulos>>> Gets()
        {
            try
            {
                return await _precioArticulosRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Compras
        [HttpGet("Reparaciones")]
        public async Task<ActionResult<IEnumerable<PreciosArticulos>>> Reparaciones()
        {
            try
            {
                return await _precioArticulosRepository.Obtener(p=> p.Id.StartsWith("00")).ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Search/{codigo}/{descripcion}")]
        public async Task<ActionResult<IEnumerable<PreciosArticulos>>> Search(string codigo, string descripcion)
        {
            try
            {
                return Ok(await _precioArticulosRepository.Search(codigo, descripcion));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Solicitud>> Get(string id)
        {
            var precioArt = await _precioArticulosRepository.ObtenerPorId(id);

            if (precioArt == null)
            {
                return NotFound();
            }

            return Ok(precioArt);
        }

        // GET: api/Compras/5
        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> Existe(string id)
        {
            bool existe = await _precioArticulosRepository.Existe(id);

            return Ok(existe);
        }

        // PUT: api/Compras/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, PreciosArticulos precioArt)
        {
            if (id != precioArt.Id)
            {
                return BadRequest();
            }

            try
            {
                await _precioArticulosRepository.Actualizar(precioArt);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _precioArticulosRepository.Existe(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok(precioArt);
        }

        // POST: api/Compras
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PreciosArticulos>> Post(PreciosArticulos precioArt)
        {
            try
            {
                await _precioArticulosRepository.Agregar(precioArt);
                return CreatedAtAction("Get", new { id = precioArt.Id }, precioArt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Compras/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PreciosArticulos>> DeleteCompra(string id)
        {
            var solicitud = await _precioArticulosRepository.ObtenerPorId(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            await _precioArticulosRepository.Remover(id);

            return solicitud;
        }
    }
}
