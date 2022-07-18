using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            return await _context.ClientesExternos.ToListAsync();
        }

        [HttpGet("Search/{idExterno}/{descripcion}")]
        public async Task<ActionResult<List<ClienteExterno>>> Search(int idExterno, string descripcion )
        {
            IQueryable<ClienteExterno> query = _context.ClientesExternos.AsQueryable();
            if (idExterno > 0)
            {
                query = _context.ClientesExternos.Where(c => c.CG_CLI == idExterno.ToString());
            }

            if (descripcion != "VACIO")
            {
                query = query.Where(c => c.DESCRIPCION.Contains(descripcion));
            }

            var clientes = await query.ToListAsync();

            return clientes;
        }
        

        // GET: api/Cliente/BuscarPorCliente/{CG_CLI}
        [HttpGet("BuscarPorCliente/{CG_CLI}")]
        public async Task<ActionResult<List<Cliente>>> BuscarPorCliente(int CG_CLI)
        {
            List<Cliente> lCliente = new List<Cliente>();
            if (_context.Cliente.Any())
            {
                lCliente = await _context.Cliente.Where(p => p.Id == CG_CLI).ToListAsync();
            }
            if (lCliente == null)
            {
                return NotFound();
            }
            return lCliente;
        }
    }
}