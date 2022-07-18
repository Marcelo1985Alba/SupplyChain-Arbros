using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Controllers;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProyectosController : ControllerBase
    {
        private readonly ProyectosGBPIRepository _proyectosRepository;

        public ProyectosController(ProyectosGBPIRepository proyectosRepository)
        {
            this._proyectosRepository = proyectosRepository;
        }

        // GET: api/Proyectos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProyectosGBPI>>> GetProyectos()
        {
            try
            {
                return await _proyectosRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Proyectos
        [HttpPost]
        public async Task<ActionResult<ProyectosGBPI>> PostProyectos(ProyectosGBPI Proyecto)
        {
            try
            {
                await _proyectosRepository.Agregar(Proyecto);
                return CreatedAtAction("GetProyectos", new { id = Proyecto.Id }, Proyecto);
            }
            catch (DbUpdateException exx)
            {
                if (!await _proyectosRepository.Existe(Proyecto.Id))
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

        /*

        // GET: api/GetProyectos
        [HttpGet("Proyectos")]
        public object GetProyectos()
        {
            IQueryable<ProyectosGBPI> data = _context.Proyectos.AsQueryable();
            return new { Items = data, Count = data.Count() };
        }
        */
    }
}