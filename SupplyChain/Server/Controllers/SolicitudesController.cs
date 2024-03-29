﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Hubs;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.DTOs;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesController : ControllerBase
    {
        private readonly SolicitudRepository _solicitudRepository;
        private readonly ServiciosRepository _serviciosRepository;
        private readonly IHubContext<SolicitudHub> _hubContext;

        public SolicitudesController(SolicitudRepository solicitudRepository,ServiciosRepository serviciosRepository, IHubContext<SolicitudHub> hubContext )
        {
            _serviciosRepository = serviciosRepository;
            _solicitudRepository = solicitudRepository;
            this._hubContext = hubContext;
        }

        // GET: api/Compras
        [HttpGet("Vista/{tipoFiltro}")]
        public async Task<ActionResult<IEnumerable<vSolicitudes>>> GetSolicitudes(TipoFiltro tipoFiltro)
        {
            try
            {
                return await _solicitudRepository.ObtenerVista(tipoFiltro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Solicitud>> GetSolicitud(int id)
        {
            try
            {
                var solicitud = await _solicitudRepository.ObtenerPorId(id);
                if (solicitud == null)
                {
                    return NotFound();
                }

                return Ok(solicitud);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Compras/5
        [HttpGet("GetSolicitudesNoPresupuestadas")]
        public async Task<ActionResult<Solicitud>> GetSolicitudesNoPresupuestadas()
        {
            var solicitud = await _solicitudRepository.Obtener(s=> !s.TienePresupuesto).ToListAsync();

            if (solicitud == null)
            {
                return NotFound();
            }

            return Ok(solicitud);
        }

        // PUT: api/Compras/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompra(int id, Solicitud solicitud)
        {
            if (id != solicitud.Id)
            {
                return BadRequest();
            }

            try
            {
                await _solicitudRepository.Actualizar(solicitud);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _solicitudRepository.Existe(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok(solicitud);
        }


        // POST: api/Compras
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("Externo")]
        public async Task<ActionResult<Solicitud>> PostCompra(SolicitudDTO solicitudDTO)
        {
            try
            {

                var solicitud = new Solicitud()
                {
                    Producto = solicitudDTO.Producto,
                    Cuit = solicitudDTO.Cuit,
                    CalcId = solicitudDTO.CalcId,
                    Cantidad = solicitudDTO.Cantidad,
                    CapacidadRequerida = solicitudDTO.CapacidadRequerida,
                    ContrapresionFija = solicitudDTO.ContrapresionFija,
                    ContrapresionVariable = solicitudDTO.ContrapresionVariable,
                    DescripcionFluido = solicitudDTO.DescripcionFluido,
                    DescripcionTag = solicitudDTO.DescripcionTag,
                    TemperaturaDescargaT = solicitudDTO.TemperaturaDescargaT,
                    Fecha = DateTime.Now,
                    PresionApertura = solicitudDTO.PresionApertura,
                };


                if (solicitud.CG_CLI == 0)
                {
                    await _solicitudRepository.AsignarClientByCuit(solicitud.Cuit, solicitud);
                }

                await _solicitudRepository.Agregar(solicitud);

                var vSolicitud = await _solicitudRepository.GetVSolicitudById(solicitud.Id);
                await _hubContext.Clients.All.SendAsync("ReceiveVSolicitud", vSolicitud);

                return CreatedAtAction("GetSolicitud", new { id = solicitud.Id }, solicitud);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        // POST: api/Compras
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Solicitud>> PostCompra(Solicitud solicitud)
        {
            try
            {
                if (solicitud.CG_CLI == 0)
                {
                    await _solicitudRepository.AsignarClientByCuit(solicitud.Cuit, solicitud);
                }
                await _solicitudRepository.Agregar(solicitud);

                
                return CreatedAtAction("GetSolicitud", new { id = solicitud.Id }, solicitud);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Compras/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Solicitud>> DeleteCompra(int id)
        {
            try
            {
                var Servicios = await _serviciosRepository.Obtener(p => p.SOLICITUD == id).FirstOrDefaultAsync();
                if (Servicios == null) return NotFound();

                //_context.Servicios.Remove(Servicios);
                //await _context.SaveChangesAsync();
                await _serviciosRepository.Remover(Servicios.Id);

            var solicitud = await _solicitudRepository.ObtenerPorId(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            await _solicitudRepository.Remover(id);

            return solicitud;
        }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
    }
           
}
    }
}
