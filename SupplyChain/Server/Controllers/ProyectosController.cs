using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Context;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class ProyectosController : ControllerBase
{
    private readonly ProyectosGBPIRepository _proyectosRepository;
    private readonly AppDbContext ganttContext;

    public ProyectosController(ProyectosGBPIRepository proyectosRepository, AppDbContext ganttContext)
    {
        _proyectosRepository = proyectosRepository;
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
        var data = _proyectosRepository.ObtenerTodosQueryable();
        return new { Items = data, Count = data.Count() };
    }
}