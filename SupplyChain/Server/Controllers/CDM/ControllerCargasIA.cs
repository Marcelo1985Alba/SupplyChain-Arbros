using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChain.Shared.CDM;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargasIAController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CargasIAController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CargasIA
        [HttpGet]
        public async Task<IEnumerable<PLANNER>> Get()
        {
            string xSQL = string.Format($"Select * From PLANNER ORDER BY CG_CELDA, ULT_ASOC");
            return await _context.PLANNER.FromSqlRaw(xSQL).ToListAsync();
        }
        
        // GET: api/CargasIA/GetCeldas
        [HttpGet("GetCeldas")]
        public async Task<IEnumerable<String>> GetCeldas()
        {
            string xSQL = string.Format($"SELECT DISTINCT CG_CELDA FROM PLANNER");
            return await _context.PLANNER.FromSqlRaw(xSQL).Select(x => x.CG_CELDA).ToListAsync();
        }
        
        // GET: api/CargasIA/GetOrdenByFecha/{fechaOrig}/{fechaDest}/{celda}
        [HttpGet("GetOrdenByFecha/{fechaOrig}/{fechaDest}/{celda}")]
        public async Task<IEnumerable<PLANNER>> GetOrdenByFecha(DateTime fecha, string celda)
        {
            string xSQL = string.Format($"SELECT * FROM PLANNER WHERE CG_CELDA = '{celda}' AND INICIO <= '{fecha}' AND FIN >= '{fecha}'");
            return await _context.PLANNER.FromSqlRaw(xSQL).ToListAsync();
        }
        
        // PUT: api/CargasIA/UpdateOrden/{CG_ORDF}
        [HttpPut("UpdateOrden/{CG_ORDF}")]
        public async Task<IActionResult> UpdateOrden(string CG_ORDF, PLANNER orden)
        {
            string xSQL = string.Format($"UPDATE PLANNER SET INICIO = '{orden.INICIO:yyyy-MM-dd HH:mm}', FIN = '{orden.FIN:yyyy-MM-dd HH:mm}' WHERE CG_ORDF = '{CG_ORDF}'");
            await _context.Database.ExecuteSqlRawAsync(xSQL);
            xSQL = string.Format($"UPDATE PROGRAMA SET FECHA_PREVISTA_FABRICACION = '{orden.INICIO:yyyy-MM-dd HH:mm}', ORDEN = ORDEN + ({orden.cambiarPrioridad}) WHERE CG_ORDF = '{CG_ORDF}'");
            await _context.Database.ExecuteSqlRawAsync(xSQL);
            return NoContent();
        }
        
        // PUT: api/CargasIA/EditOrden/{CG_ORDF}
        [HttpPut("EditOrden/{CG_ORDF}")]
        public async Task<IActionResult> EditOrden(string CG_ORDF, PLANNER orden)
        {
            string xSQL = string.Format($"UPDATE PLANNER SET CG_ESTADOCARGA = '{orden.CG_ESTADOCARGA}', CG_CELDA = '{orden.CG_CELDA}', PRIORIDAD = '{orden.PRIORIDAD}' WHERE CG_ORDF = '{CG_ORDF}'");
            await _context.Database.ExecuteSqlRawAsync(xSQL);
            return NoContent();
        }
    }
}