using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;
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
        private readonly Repositorios.SolicitudRepository _solicitudRepository;

        public SolicitudesController(Repositorios.SolicitudRepository solicitudRepository)
        {
            _solicitudRepository = solicitudRepository;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<vSolicitudes>>> GetSolicitudes()
        {
            //OC ABIERTAS
            try
            {
                return await _solicitudRepository.ObtenerTodosFromVista();
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
            var solicitud = await _solicitudRepository.ObtenerPorId(id);

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

            return NoContent();
        }

        // POST: api/Compras
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Solicitud>> PostCompra(Solicitud solicitud)
        {
            await _solicitudRepository.AsignarClientByCuit(solicitud.Cuit, solicitud);
            await _solicitudRepository.Agregar(solicitud);

            return CreatedAtAction("GetSolicitud", new { id = solicitud.Id }, solicitud);
        }

        // DELETE: api/Compras/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Solicitud>> DeleteCompra(int id)
        {
            var solicitud = await _solicitudRepository.ObtenerPorId(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            await _solicitudRepository.Remover(id);

            return solicitud;
        }
    }
}
