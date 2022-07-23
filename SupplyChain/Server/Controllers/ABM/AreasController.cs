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
    public class AreasController : ControllerBase
    {
        private readonly AreasRepository _areasRepository;

        public AreasController(AreasRepository areasRepository)
        {
            this._areasRepository = areasRepository;
        }

        // GET: api/Areas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Areas>>> GetAreas()
        {
            try
            {
                return await _areasRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteArea(string id)
        {
            try
            {
                return await _areasRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Areas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAreas(int id, Areas Area)
        {
            if (id != Area.Id)
            {
                return BadRequest();
            }

            try
            {
                await _areasRepository.Actualizar(Area);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _areasRepository.Existe(id))
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
            return Ok(Area);
        }

        // POST: api/Areas
        [HttpPost]
        public async Task<ActionResult<Areas>> PostAreas(Areas Area)
        {
            try
            {
                await _areasRepository.Agregar(Area);
                return CreatedAtAction("GetAreas", new { id = Area.Id }, Area);
            }
            catch (DbUpdateException exx)
            {
                if (!await _areasRepository.Existe(Area.Id))
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

        // DELETE: api/Areas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Areas>> DeleteAreas(int id)
        {
            var Area = await _areasRepository.ObtenerPorId(id);
            if (Area == null)
            {
                return NotFound();
            }

            await _areasRepository.Remover(id);

            return Area;
        }

        [HttpPost("PostList")]
        public async Task<ActionResult<Areas>> PostList(List<Areas> areas)
        {
            try
            {
                foreach (var item in areas)
                {
                    await _areasRepository.Remover(item.Id);
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