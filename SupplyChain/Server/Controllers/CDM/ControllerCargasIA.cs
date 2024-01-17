using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
            string xSQL = string.Format($"Select * From PLANNER ORDER BY CG_CELDA, INICIO");
            return await _context.PLANNER.FromSqlRaw(xSQL).ToListAsync();
        }
        
        // GET: api/CargasIA/GetCeldas
        [HttpGet("GetCeldas")]
        public async Task<IEnumerable<String>> GetCeldas()
        {
            string xSQL = string.Format($"SELECT DISTINCT CG_CELDA FROM PLANNER");
            return await _context.PLANNER.FromSqlRaw(xSQL).Select(x => x.CG_CELDA).ToListAsync();
        }
    }
}