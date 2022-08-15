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
    public class MantCeldasController : ControllerBase
    {
        private readonly MantCeldasRepository _mantCeldasRepository;

        public MantCeldasController(MantCeldasRepository mantCeldasRepository)
        {
            this._mantCeldasRepository = mantCeldasRepository;
        }

        // GET: api/MantCeldas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MantCeldas>>> GetMantCeldas()
        {
            try
            {
                return await _mantCeldasRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteMantCelda(int id)
        {
            try
            {
                return await _mantCeldasRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/MantCeldas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMantCeldas(int id, MantCeldas MantCelda)
        {
            if (id != MantCelda.Id)
            {
                return BadRequest();
            }

            try
            {
                await _mantCeldasRepository.Actualizar(MantCelda);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _mantCeldasRepository.Existe(id))
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
            return Ok(MantCelda);
        }

        // POST: api/MantCeldas
        [HttpPost]
        public async Task<ActionResult<MantCeldas>> PostMantCeldas(MantCeldas MantCelda)
        {
            try
            {
                await _mantCeldasRepository.Agregar(MantCelda);
                return CreatedAtAction("GetMantCeldas", new { id = MantCelda.Id }, MantCelda);
            }
            catch (DbUpdateException exx)
            {
                if (!await _mantCeldasRepository.Existe(MantCelda.Id))
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

        // DELETE: api/MantCeldas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MantCeldas>> DeleteMantCeldas(int id)
        {
            var MantCelda = await _mantCeldasRepository.ObtenerPorId(id);
            if (MantCelda == null)
            {
                return NotFound();
            }

            await _mantCeldasRepository.Remover(id);

            return MantCelda;
        }

        [HttpPost("PostList")]
        public async Task<ActionResult<MantCeldas>> PostList(List<MantCeldas> mantCeldas)
        {
            try
            {
                foreach (var item in mantCeldas)
                {
                    await _mantCeldasRepository.Remover(item.Id);
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