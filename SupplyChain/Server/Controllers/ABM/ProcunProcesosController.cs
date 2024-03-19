using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers.ABM
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcunProcesosController : ControllerBase
    {
        private readonly ProcunProcesoRepository _procunProcesoRepository;

        public ProcunProcesosController(ProcunProcesoRepository procunProcesoRepository)
        {
            this._procunProcesoRepository=procunProcesoRepository;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcunProceso>>> GetProcunProcesos()
        {
            try
            {
                return await _procunProcesoRepository.ObtenerTodos();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }

        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> GetProcunProceso(int id)
        {
            try
            {
                return await _procunProcesoRepository.Existe(id);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcunProcedimiento(int id, ProcunProceso procunProceso)
        {
            if(id != procunProceso.Id)
            {
                return BadRequest();
            }

            try
            {
                await _procunProcesoRepository.Actualizar(procunProceso);
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!await _procunProcesoRepository.Existe(id))
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
                return BadRequest(ex.Message);
            }
            return Ok(procunProceso);
        }

        [HttpPost]
        public async Task<ActionResult<ProcunProceso>> PostProcunProceso(ProcunProceso procunProceso)
        {
            try
            {
                await _procunProcesoRepository.Agregar(procunProceso);
                return CreatedAtAction("GetProcunProcesos", new { id = procunProceso.Id }, procunProceso);
            }
            catch(DbUpdateConcurrencyException exx) 
            {
                if(!await _procunProcesoRepository.Existe(procunProceso.Id))
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
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProcunProceso>> DeleteProcunProcedimiento(int id)
        {
            var procunProceso = await _procunProcesoRepository.ObtenerPorId(id);
            if (procunProceso == null)
            {
                return NotFound();
            }

            await _procunProcesoRepository.Remover(id);

            return procunProceso;
        }

        [HttpPost("PostList")]
        public async Task<ActionResult<TipoArea>> PostList(List<ProcunProceso> procunProcesos)
        {
            try
            {
                foreach (var item in procunProcesos)
                {
                    await _procunProcesoRepository.Remover(item.Id);
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

