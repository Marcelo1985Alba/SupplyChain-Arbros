using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SupplyChain.Server.Controllers;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Context;
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
        private readonly GanttContext ganttContext;

        public ProyectosController(ProyectosGBPIRepository proyectosRepository, GanttContext ganttContext)
        {
            this._proyectosRepository = proyectosRepository;
            this.ganttContext = ganttContext;
        }

        //GET: api/Proyectos
       [HttpGet]
        public async Task<ActionResult<IEnumerable<GanttDataDetails>>> GetProyectos()
        {
            try
            {
                var proyectos = await ganttContext.GanttData.ToListAsync();
                return proyectos;
                //return await _proyectosRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("id")]
        public async Task<ActionResult<GanttDataDetails>> GetProyecto(int id)
        {
            try
            {
                return await ganttContext.GanttData.FindAsync(id);
                //return await _proyectosRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Proyectos
        [HttpPost]
        public async Task<ActionResult<GanttDataDetails>> PostProyectos(GanttDataDetails proyecto)
        {
            try
            {
                ganttContext.GanttData.Add(proyecto);
                await ganttContext.SaveChangesAsync();
                return CreatedAtAction("GetProyecto", new { id = proyecto.Id }, proyecto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Proyectos/Proyectos
        [HttpGet("Proyectos")]
        public object GetProyectosQueryable()
        {
            IQueryable<ProyectosGBPI> data = _proyectosRepository.ObtenerTodosQueryable();
            return new { Items = data, Count = data.Count() };
        }

    }
}