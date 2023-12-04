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

        // GET: api/CargasIA/GetCN1
        [HttpGet("GetCN1")]
        public async Task<IEnumerable<PLANNER_CN1>> GetCN1()
        {
            string xSQL = string.Format($"Select * From PLANNER_CN1");
            return await _context.PLANNER_CN1.FromSqlRaw(xSQL).ToListAsync();
        }
        
        // GET: api/CargasIA/GetCN2
        [HttpGet("GetCN2")]
        public async Task<IEnumerable<PLANNER_CN1>> GetCN2()
        {
            string xSQL = string.Format($"Select * From PLANNER_CN2");
            return await _context.PLANNER_CN1.FromSqlRaw(xSQL).ToListAsync();
        }
    }
}