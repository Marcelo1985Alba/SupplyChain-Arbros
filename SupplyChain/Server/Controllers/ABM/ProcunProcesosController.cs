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
        public async Task<ActionResult<IEnumerable<Protab>>> GetProcunProcesos()
        {
            try
            {
                var listapro = await _procunProcesoRepository.ObtenerTodos();
                return Ok(listapro);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }

        }

        [HttpGet("id")]
        public async Task<ActionResult<Protab>>GetProcunProcesosById(string id)
        {
            try
            {
                return await _procunProcesoRepository.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> GetProcunProceso(string id)
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
        public async Task<ActionResult<Protab>> PutProcunProcedimiento(decimal id, Protab protab)
        {
            
            try
            {
                await _procunProcesoRepository.Actualizar(protab);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _procunProcesoRepository.Existe(id))
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
                return BadRequest(ex.Message);
            }
            return Ok(protab);
        }

       
        [HttpPost]
        public async Task<ActionResult<Protab>> PostProcunProcesos(Protab protab)
        {
            try
            {
                await _procunProcesoRepository.Agregar(protab);
                return CreatedAtAction("GetProcunProcesos", new { id = protab.Id }, protab);
            }
            catch(DbUpdateConcurrencyException exx) 
            {
                if(!await _procunProcesoRepository.Existe(protab.Id))
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
        public async Task<ActionResult<Protab>> DeleteProcunProcedimiento(string id)
        {
            try
            {
                var procunProceso = await _procunProcesoRepository.Obtener(p=>p.Id == id).FirstOrDefaultAsync();
                if (procunProceso == null)
                {
                    return NotFound();
                }
                await _procunProcesoRepository.Remover(id);
                return procunProceso;

            }
            catch(Exception ex)
            {
                return BadRequest("Error al eliminar el proceso" + ex.Message);

            }
        }

        [HttpPost("PostList")]
        public async Task<ActionResult<Protab>> PostList(List<Protab> protab)
        {
            try
            {
                foreach (var item in protab)
                {
                    await _procunProcesoRepository.Remover(item.Id.Trim());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

    }
}

