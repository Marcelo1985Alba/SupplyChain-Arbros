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
    public class TipoAreaController : ControllerBase
    {
        private readonly TipoAreaRepository _tipoAreaRepository;

        public TipoAreaController(TipoAreaRepository tipoAreaRepository)
        {
            this._tipoAreaRepository = tipoAreaRepository;
        }

        // GET: api/TipoArea
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoArea>>> GetTipoAreas()
        {
            try
            {
                return await _tipoAreaRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/TipoArea/Existe/{id}
        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteTipoArea(int id)
        {
            try
            {
                return await _tipoAreaRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/TipoArea/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoAreas(int id, TipoArea tipoArea)
        {
            if (id != tipoArea.Id)
            {
                return BadRequest();
            }

            try
            {
                await _tipoAreaRepository.Actualizar(tipoArea);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _tipoAreaRepository.Existe(id))
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
            return Ok(tipoArea);
        }

        // POST: api/TipoArea
        [HttpPost]
        public async Task<ActionResult<TipoArea>> PostTipoAreas(TipoArea tipoArea)
        {
            try
            {
                await _tipoAreaRepository.Agregar(tipoArea);
                return CreatedAtAction("GetTipoAreas", new { id = tipoArea.Id }, tipoArea);
            }
            catch (DbUpdateException exx)
            {
                if (!await _tipoAreaRepository.Existe(tipoArea.Id))
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

        // DELETE: api/TipoArea/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<TipoArea>> DeleteTipoAreas(int id)
        {
            var tipoArea = await _tipoAreaRepository.ObtenerPorId(id);
            if (tipoArea == null)
            {
                return NotFound();
            }

            await _tipoAreaRepository.Remover(id);

            return tipoArea;
        }

        // POST: api/TipoArea/PostList
        [HttpPost("PostList")]
        public async Task<ActionResult<TipoArea>> PostList(List<TipoArea> tipoAreas)
        {
            try
            {
                foreach (var item in tipoAreas)
                {
                    await _tipoAreaRepository.Remover(item.Id);
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