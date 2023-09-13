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

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolutionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SolutionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Solution>>> Get()
        {
            try
            {
                string xSQL = string.Format("select CAMPO, VALORC from Solution where CAMPO IN ( 'RUTAOF', 'RUTACNC','RUTAENSAYO'," +
                    " 'RUTAOF2','RUTATRAZABILIDAD', 'RUTADATOS')");
                return await _context.Solution.FromSqlRaw(xSQL).ToListAsync();
            }
            catch(Exception ex)
            {
                BadRequest(ex);
                return new List<Solution>();
            }
        }


        [HttpGet("campo")]
        public async Task<ActionResult<Solution>> GetByCampo(string campo)
        {
            try
            {
                return await _context.Solution.Where(s=> s.CAMPO == campo).FirstOrDefaultAsync();
            }
            catch
            {
                return BadRequest();
            }
        }


    }
}