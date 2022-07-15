using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresupuestosController : ControllerBase
    {
        private readonly PresupuestoAnteriorRepository _presupuestoAnteriorRepository;
        private readonly PresupuestoRepository _presupuestoRepository;
        private readonly GeneraRepository _generaRepository;

        public PresupuestosController(PresupuestoAnteriorRepository presupuestoAnteriorRepository, 
            PresupuestoRepository presupuestoRepository, 
            GeneraRepository generaRepository)
        {
            _presupuestoAnteriorRepository = presupuestoAnteriorRepository;
            _presupuestoRepository = presupuestoRepository;
            this._generaRepository = generaRepository;
        }
        // GET: api/<PresupuestosController>
        [HttpGet]
        public async Task<List<Presupuesto>> GetPresupuesto()
        {
            return await _presupuestoRepository.ObtenerTodosQueryable().Include(p=> p.Items).ToListAsync();
        }

        [HttpGet("GetPresupuestoVista")]
        public async Task<List<vPresupuestos>> GetPresupuestoVista()
        {
            return await _presupuestoRepository.GetForView();
        }

        // GET api/<PresupuestosController>/5
        [HttpGet("{id}")]
        public async Task<Presupuesto> GetPresupuesto(int id)
        {
            var presup = await _presupuestoRepository.Obtener(p => p.Id == id).Include(p => p.Items)
                //.ThenInclude(i=> i.Solicitud)
                .FirstOrDefaultAsync();

            
            await _presupuestoRepository.AgregarDatosFaltantes(presup);


            return presup;
        }

        // POST api/<PresupuestosController>
        [HttpPost]
        public async Task<ActionResult<Presupuesto>> Post(Presupuesto presupuesto)
        {
            return await AgregarNuevoPresupuesto(presupuesto);
        }

        private async Task<ActionResult<Presupuesto>> AgregarNuevoPresupuesto(Presupuesto presupuesto)
        {
            foreach (var item in presupuesto.Items)
            {
                item.Id = 0;
            }
            await _presupuestoRepository.BeginTransaction();
            try
            {
                await _presupuestoRepository.Agregar(presupuesto);
                await _presupuestoRepository.ActualizarCalculoConPresupuestoByIdCalculo(presupuesto.Id);
                await _presupuestoRepository.CommitTransaction();
                
            }
            catch (Exception ex)
            {
                await _presupuestoRepository.RollbackTransaction();
                return BadRequest(ex.Message);
            }

            return CreatedAtAction("GetPresupuesto", new { id = presupuesto.Id }, presupuesto);
        }

        [HttpPost("PostFromSolicitud")]
        public async Task<ActionResult<Presupuesto>> PostFromSolicitud(Presupuesto presupuesto)
        {
            try
            {
                await _presupuestoRepository.AgregarDatosFaltantes(presupuesto);

                return await AgregarNuevoPresupuesto(presupuesto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/<PresupuestosController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Presupuesto>> Put(int id, Presupuesto presupuesto)
        {
            try
            {
                await _presupuestoRepository.Actualizar(presupuesto);
                await _presupuestoRepository.AgregarEliminarActualizarDetalles(presupuesto.Items);
                await _presupuestoRepository.ActualizarCalculoConPresupuestoByIdCalculo(presupuesto.Id);
                return Ok(presupuesto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE api/<PresupuestosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
