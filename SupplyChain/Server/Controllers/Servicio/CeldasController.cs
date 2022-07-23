using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Models;
using SupplyChain.Shared;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class CeldasController : ControllerBase
    {
        private readonly CeldasRepository _celdasRepository;

        public CeldasController(CeldasRepository celdasRepository)
        {
            this._celdasRepository = celdasRepository;
        }

        // GET: api/Celdas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Celdas>>> GetCeldas()
        {
            try
            {
                return await _celdasRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteCelda(string id)
        {
            try
            {
                return await _celdasRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Celdas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCeldas(string id, Celdas Celda)
        {
            if (id != Celda.Id)
            {
                return BadRequest();
            }

            try
            {
                await _celdasRepository.Actualizar(Celda);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _celdasRepository.Existe(id))
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
            return Ok(Celda);
        }

        // POST: api/Celdas
        [HttpPost]
        public async Task<ActionResult<Celdas>> PostCeldas(Celdas Celda)
        {
            try
            {
                await _celdasRepository.Agregar(Celda);
                return CreatedAtAction("GetCeldas", new { id = Celda.Id }, Celda);
            }
            catch (DbUpdateException exx)
            {
                if (!await _celdasRepository.Existe(Celda.Id))
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

        // DELETE: api/Celdas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Celdas>> DeleteCeldas(string id)
        {
            var Celda = await _celdasRepository.ObtenerPorId(id);
            if (Celda == null)
            {
                return NotFound();
            }

            await _celdasRepository.Remover(id);

            return Celda;
        }

        [HttpPost("PostList")]
        public async Task<ActionResult<Celdas>> PostList(List<Celdas> celdas)
        {
            try
            {
                foreach (var item in celdas)
                {
                    await _celdasRepository.Remover(item.Id.Trim());
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