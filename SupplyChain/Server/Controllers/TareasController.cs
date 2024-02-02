using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SupplyChain.Shared;
using SupplyChain.Shared.CDM;

namespace SupplyChain
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        [CascadingParameter] public Task<AuthenticationState> authenticationState { get; set; }
        [Inject] protected HttpClient Http { get; set; }

        private readonly AppDbContext _context;

        public TareasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tareas
        [HttpGet]
        public async Task<IEnumerable<Tareas>> Get()
        {
            string xSQL = string.Format($"Select * From Tareas");
            return await _context.Tareas.FromSqlRaw(xSQL).ToListAsync();
        }

        // GET: api/Tareas/TareasByModulos
        [HttpGet("TareasByModulos")]
        public async Task<IEnumerable<Tareas>> GetTareasByModulos([FromQuery] string modulos)
        {    
            // devuelvo las tareas en donde el modulo sea igual a alguno de los modulos de la lista
            string xSQL = $"Select * From Tareas Where Modulo IN ('{modulos}')";
            return await _context.Tareas.FromSqlRaw(xSQL).ToListAsync();
        }
        
        // GET: api/Tareas/TareasByUsuarios
        [HttpGet("TareasByUsuarios")]
        public async Task<IEnumerable<TareasPorUsuario>> GetTareasByUsuarios([FromQuery] string userId)
        {    
            string xSQL = $"Select * From TareasPorUsuario Where userId = '{userId}'";
            return await _context.TareasPorUsuario.FromSqlRaw(xSQL).ToListAsync();
        }
        
        // GET: api/Tareas/TareasByTaskId
        [HttpGet("TareasByTaskId")]
        public async Task<IEnumerable<TareasPorUsuario>> GetTareasByTaskId([FromQuery] string taskId)
        {    
            string xSQL = $"Select * From TareasPorUsuario Where tareaId IN ('{taskId}')";
            return await _context.TareasPorUsuario.FromSqlRaw(xSQL).ToListAsync();
        }
        
        // GET: api/Tareas/GetTareaPorUsuarioByUserAndTask/{tareaId}
        [HttpGet("GetTareaPorUsuarioByUserAndTask/{tareaId}")]
        public async Task<TareasPorUsuario> GetTareaPorUsuarioByUserAndTask([FromQuery] string userId, int tareaId)
        {
            string xSQL = $"Select * From TareasPorUsuario Where userId = '{userId}' AND tareaId = '{tareaId}'";
            return await _context.TareasPorUsuario.FromSqlRaw(xSQL).FirstOrDefaultAsync();
        }
        
        // PUT: api/Tareas/UpdateTarea/{Id}
        [HttpPut]
        public async Task<IActionResult> UpdateTarea(Tareas tarea)
        {
            string xSQL = string.Format($"UPDATE Tareas SET Titulo = '{tarea.Titulo}', Estado = '{tarea.Estado}', Resumen = '{tarea.Resumen}', Modulo = '{tarea.Modulo}', FechaRequerida = '{tarea.FechaRequerida:MM/dd/yyyy}', Importancia = '{tarea.Importancia}' WHERE Id = '{tarea.Id}'");
            await _context.Database.ExecuteSqlRawAsync(xSQL);
            return NoContent();
        }
        
        // POST: api/Tareas/InsertTarea
        [HttpPost]
        public async Task<ActionResult<Tareas>> InsertTarea(Tareas tarea)
        {
            string xSQL = string.Format($"INSERT INTO Tareas (Titulo, Estado, Resumen, Modulo, FechaRequerida, Importancia, Creador) VALUES ('{tarea.Titulo}', '{tarea.Estado}', '{tarea.Resumen}', '{tarea.Modulo}', '{tarea.FechaRequerida:MM/dd/yyyy}', '{tarea.Importancia}', '{tarea.Creador}')");
            await _context.Database.ExecuteSqlRawAsync(xSQL);
            return CreatedAtAction("Get", new { id = tarea.Id }, tarea);
        }
        
        // POST: api/Tareas/TomarTarea
        [HttpPost("TomarTarea")]
        public async Task<ActionResult<TareasPorUsuario>> TomarTarea(TareasPorUsuario tareaPorUsuario)
        {
            string xSQL = string.Format($"INSERT INTO TareasPorUsuario (tareaId, userId) VALUES ('{tareaPorUsuario.tareaId}','{tareaPorUsuario.userId}')");
            await _context.Database.ExecuteSqlRawAsync(xSQL);
            return CreatedAtAction("Get", new { id = tareaPorUsuario.tareaId }, tareaPorUsuario);
        }
        
        // DELETE: api/Tareas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DejarTarea(int id)
        {
            string xSQL = string.Format($"DELETE FROM TareasPorUsuario WHERE Id = '{id}'");
            await _context.Database.ExecuteSqlRawAsync(xSQL);
            return NoContent();
        }
    }
}