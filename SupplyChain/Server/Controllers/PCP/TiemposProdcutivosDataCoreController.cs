using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared.PCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SupplyChain.Server.Controllers.PCP
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiemposProdcutivosDataCoreController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TiemposProdcutivosDataCoreController( AppDbContext context)
        {
            _context = context;
        }



        // GET: api/TiemposProdcutivosDataCoreController
        [HttpGet]
        public async Task<ActionResult<IEnumerable<vProdMaquinaDataCore>>> Get()
        {
            try
            {
                return await _context.VProdMaquinaDataCore.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<TeimposProdcutivosDataCoreController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
