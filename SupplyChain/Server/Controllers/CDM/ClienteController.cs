using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> Get()
        {
            List<Cliente> lCliente = new();
            return await _context.Cliente.ToListAsync();
        }

        [HttpGet("GetClienteExterno")]
        public async Task<ActionResult<List<ClienteExterno>>> GetClienteExterno()
        {
            List<ClienteExterno> lCliente = new();
            return await _context.ClientesExternos.ToListAsync();
        }


        // GET: api/Cliente/BuscarPorCliente/{CG_CLI}
        [HttpGet("BuscarPorCliente/{CG_CLI}")]
        public async Task<ActionResult<List<Cliente>>> BuscarPorCliente(int CG_CLI)
        {
            List<Cliente> lCliente = new List<Cliente>();
            if (_context.Cliente.Any())
            {
                lCliente = await _context.Cliente.Where(p => p.CG_CLI == CG_CLI).ToListAsync();
            }
            if (lCliente == null)
            {
                return NotFound();
            }
            return lCliente;
        }
    }
}