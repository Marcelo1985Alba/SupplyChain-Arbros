using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ProporcionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProporcionController(AppDbContext context)
        {
            _context=context;
        }
        
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proporcion>>> GetProporcionTodos()
        {
            try
            {
                return await _context.Proporcion.ToListAsync();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("proporC")]
        public async Task<ActionResult<Proporcion>> GetProporcion(string proporc)
        {
            var pro = await _context.Proporcion.FindAsync(proporc);
            if (pro == null)
            {
                return NotFound();
            }
            return pro;
        }
    }
}
