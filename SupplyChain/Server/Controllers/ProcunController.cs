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
    public class ProcunController : ControllerBase
    {
        private readonly ProcunRepository _procunRepository;

        public ProcunController(ProcunRepository procunRepository)
        {
            this._procunRepository = procunRepository;
        }

        // GET: api/Procun
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Procun>>> GetProcun()
        {
            try
            {
                return await _procunRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteProcun(int id)
        {
            try
            {
                return await _procunRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Procun/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcun(int id, Procun Proc)
        {
            if (id != Proc.Id)
            {
                return BadRequest();
            }

            try
            {
                await _procunRepository.Actualizar(Proc);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _procunRepository.Existe(id))
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
            return Ok(Proc);
        }

        // POST: api/Procun
        [HttpPost]
        public async Task<ActionResult<Procun>> PostProcun(Procun Proc)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                Proc.USUARIO=username;
                Proc.AUTORIZA=username;
                await _procunRepository.Agregar(Proc);
                return CreatedAtAction("GetProcun", new { id = Proc.Id }, Proc);
            }
            catch (DbUpdateException exx)
            {
                if (!await _procunRepository.Existe(Proc.Id))
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

        // DELETE: api/Procun/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Procun>> DeleteProcun(int id)
        {
            var Proc = await _procunRepository.ObtenerPorId(id);
            if (Proc == null)
            {
                return NotFound();
            }

            await _procunRepository.Remover(id);

            return Proc;
        }

        [HttpPost("PostList")]
        public async Task<ActionResult<Procun>> PostList(List<Procun> Procs)
        {
            try
            {
                foreach (var item in Procs)
                {
                    await _procunRepository.Remover(item.Id);
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