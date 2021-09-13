﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ServiciosController : ControllerBase
    {
        //private IHostingEnvironment hostingEnv;
        private readonly AppDbContext _context;

        public ServiciosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> Get()
        {
            var xitem = await _context.Servicios.OrderByDescending(s=> s.PEDIDO).ToListAsync();
            
            return xitem;
        }

        // GET: api/Servicios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServicios(string id)
        {
            var Servicios = await _context.Servicios.Where(s=> s.PEDIDO == id).ToListAsync();

            if (Servicios == null)
            {
                return NotFound();
            }

            return Servicios;
        }

        // PUT: api/Servicios/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicios(string id, Service Servicios)
        {
            if (id != Servicios.PEDIDO)
            {
                return BadRequest();
            }

            _context.Entry(Servicios).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!ServiciosExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Servicios
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Service>> PostServicios(Service Servicios)
        {
            _context.Servicios.Add(Servicios);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ServiciosExists(Servicios.PEDIDO))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetServicios", new { id = Servicios.PEDIDO }, Servicios);
        }

        // DELETE: api/Servicios/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Service>> DeleteServicios(string id)
        {
            var Servicios = await _context.Servicios.FindAsync(id);
            if (Servicios == null)
            {
                return NotFound();
            }

            _context.Servicios.Remove(Servicios);
            await _context.SaveChangesAsync();

            return Servicios;
        }

        private bool ServiciosExists(string id)
        {
            return _context.Servicios.Any(e => e.PEDIDO == id);
        }
    }
}