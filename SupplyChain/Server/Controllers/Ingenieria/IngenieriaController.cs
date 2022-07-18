using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers.Ingenieria
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngenieriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IngenieriaController(AppDbContext appDbContext)
        {
            this._context = appDbContext;
        }

        // GET: api/Ingenieria/GetProductoFormulas
        [HttpGet("GetProductoFormulas")]
        public async Task<ActionResult<IEnumerable<vIngenieriaProductosFormulas>>> GetProductoFormulas()
        {
            //OC ABIERTAS
            try
            {
                return await _context.vIngenieriaProductosFormulas.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
