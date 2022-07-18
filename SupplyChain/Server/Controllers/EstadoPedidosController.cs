using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoPedidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstadoPedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Genera
        [HttpGet]
        public async Task<ActionResult<IEnumerable<vEstadoPedido>>> GetGenera()
        {
            return await _context.vEstadoPedidos.ToListAsync();
        }
    }
}
