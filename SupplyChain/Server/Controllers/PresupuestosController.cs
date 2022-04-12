﻿using Microsoft.AspNetCore.Mvc;
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
        

        private readonly PresupuestoRepository _presupuestoRepository;
        private readonly GeneraRepository _generaRepository;

        public PresupuestosController(PresupuestoRepository presupuestoRepository, GeneraRepository generaRepository)
        {
            _presupuestoRepository = presupuestoRepository;
            this._generaRepository = generaRepository;
        }
        // GET: api/<PresupuestosController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PresupuestosController>/5
        [HttpGet("{id}")]
        public async Task<Presupuesto> GetPresupuesto(int id)
        {
            return await _presupuestoRepository.Obtener(p=> p.PRESUP == id).FirstOrDefaultAsync();
        }

        // POST api/<PresupuestosController>
        [HttpPost]
        public async Task<ActionResult<Presupuesto>> Post(Presupuesto presupuesto)
        {
            return await AgregarNuevoPresupuesto(presupuesto);
        }

        private async Task<ActionResult<Presupuesto>> AgregarNuevoPresupuesto(Presupuesto presupuesto)
        {
            await _presupuestoRepository.Agregar(presupuesto);
            
            return CreatedAtAction("GetPresupuesto", new { id = presupuesto.PRESUP }, presupuesto);
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
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PresupuestosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
