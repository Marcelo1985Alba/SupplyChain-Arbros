using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;

using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers.ABM
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcedimientosController : ControllerBase
    {
        private readonly ProcedimientosRepository _procedimientosRepository;

        public ProcedimientosController(ProcedimientosRepository procedimientosRepository)
        {
            this._procedimientosRepository = procedimientosRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Operaciones>>> GetProcedimientos()
        {
            try
            {
                return await _procedimientosRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteProcedimientos(string id)
        {
            try
            {
                return await _procedimientosRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ExisteProcedimiento/{id}")]
        public async Task<ActionResult<bool>> ExisteProcedimiento(string id)
        {
            var existe = await _procedimientosRepository.Existe(id);
            return existe;
        }

        [HttpGet("BuscarProcedimiento/{CG_PROD}/{Busqueda}")]
        public async Task<ActionResult<List<Operaciones>>> BuscarProcedimiento(string CG_PROD, int Busqueda)
        {
            List<Operaciones> operaciones = new();
            if ((string.IsNullOrEmpty(CG_PROD)))
            {
                operaciones = (await _procedimientosRepository.ObtenerTodos()).Take(Busqueda).ToList();
            }
            else if (string.IsNullOrEmpty(CG_PROD) || CG_PROD == "Vacio")
            {
                operaciones = await _procedimientosRepository.Obtener(p => p.Id.Equals(CG_PROD), Busqueda).ToListAsync();
                if (operaciones == null)
                {
                    return NotFound();
                }
            }
            return operaciones;
        }

        [HttpPut]
        public async Task<IActionResult> PutProc(Operaciones operaciones)
        {
            try
            {
                await _procedimientosRepository.Actualizar(operaciones);

            }
            catch(DbUpdateConcurrencyException)
            {
                if(!await _procedimientosRepository.Existe(operaciones.Id))
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
            return Ok(operaciones);
        }

        [HttpPost]
        public async Task<ActionResult<Operaciones>> PostProc(Operaciones operaciones)
        {
            try
            {
                await _procedimientosRepository.Agregar(operaciones);
                return CreatedAtAction("GetProcedimientos", new { id = operaciones.Id }, operaciones);
            }
            catch(DbUpdateException ex)
            {
                if(!await _procedimientosRepository.Existe(operaciones.Id))
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

        [HttpPost("PostList")]
        public async Task<ActionResult<List<Operaciones>>> PostList(List<Operaciones> operaciones)
        {
            try
            {
                foreach(var item in operaciones)
                {
                    await _procedimientosRepository.Remover(item.Id);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new List<Operaciones>());
        }

        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Operaciones>>DeleteOperaciones(int id)
        //{
        //    var operaciones= await _procedimientosRepository.ObtenerPorId(id);
        //    if(operaciones == null)
        //    {
        //        return BadRequest();
        //    }

        //    await _procedimientosRepository.Remover(id);
        //    return operaciones;
        //}
    }
}