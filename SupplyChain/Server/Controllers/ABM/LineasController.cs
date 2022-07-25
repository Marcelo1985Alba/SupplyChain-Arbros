using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineasController : ControllerBase
    {
        private readonly LineasRepository _lineasRepository;

        public LineasController(LineasRepository lineasRepository)
        {
            this._lineasRepository = lineasRepository;
        }

        // GET: api/Lineas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lineas>>> GetLineas()
        {
            try
            {
                return await _lineasRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Lineas/Existe/{id}
        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteLinea(int id)
        {
            try
            {
                return await _lineasRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Lineas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLineas(int id, Lineas Linea)
        {
            if (id != Linea.Id)
            {
                return BadRequest();
            }

            try
            {
                await _lineasRepository.Actualizar(Linea);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _lineasRepository.Existe(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok(Linea);
        }

        // POST: api/Lineas
        [HttpPost]
        public async Task<ActionResult<Lineas>> PostLineas(Lineas Linea)
        {
            try
            {
                await _lineasRepository.Agregar(Linea);
                return CreatedAtAction("GetLineas", new { id = Linea.Id }, Linea);
            }
            catch (DbUpdateException exx)
            {
                if (!await _lineasRepository.Existe(Linea.Id))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Lineas/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Lineas>> DeleteLineas(int id)
        {
            var Linea = await _lineasRepository.ObtenerPorId(id);
            if (Linea == null)
            {
                return NotFound();
            }

            await _lineasRepository.Remover(id);

            return Linea;
        }

        // POST: api/Lineas/PostList
        [HttpPost("PostList")]
        public async Task<ActionResult<Lineas>> PostList(List<Lineas> lineas)
        {
            try
            {
                foreach (var item in lineas)
                {
                    await _lineasRepository.Remover(item.Id);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}