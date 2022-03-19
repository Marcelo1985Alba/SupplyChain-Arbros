﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly ServiciosRepository _serviciosRepository;

        public ServiciosController(ServiciosRepository serviciosRepository)
        {
            this._serviciosRepository = serviciosRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> Get()
        {
            var xitem = await _serviciosRepository.ObtenerTodos();
                
            return xitem.OrderByDescending(s => s.PEDIDO).ToList();
        }

        // GET: api/Servicios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Service>>> Get(string id)
        {
            try
            {
                var Servicios = await _serviciosRepository.Obtener(s => s.PEDIDO == id).ToListAsync();

                return Servicios == null ? NotFound() : Ok(Servicios);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Servicios/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicios(string id, Service Servicios)
        {
            if (id != Servicios.PEDIDO) return BadRequest();

            try
            {
                await _serviciosRepository.Actualizar(Servicios);
            }
            catch (Exception ex)
            {
                if (!await _serviciosRepository.Existe(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok();
        }


        // POST: api/Servicios
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Service>> PostServicios(Service Servicios)
        {
            try
            {
                await _serviciosRepository.Agregar(Servicios);
            }
            catch (DbUpdateException)
            {
                if (await _serviciosRepository.Existe(Servicios.PEDIDO))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest();
                }
            }

            return CreatedAtAction("Get", new { id = Servicios.PEDIDO }, Servicios);
        }

        // DELETE: api/Servicios/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Service>> DeleteServicios(string id)
        {
            var Servicios = await _serviciosRepository.ObtenerPorId(id);
            if (Servicios == null) return NotFound();

            //_context.Servicios.Remove(Servicios);
            //await _context.SaveChangesAsync();
            await _serviciosRepository.Remover(id);

            return Ok(Servicios);
        }

    }
}