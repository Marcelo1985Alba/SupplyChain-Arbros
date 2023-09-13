using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteRepository _clienteRepository;
        private readonly ClienteExternoRepository _clienteExternoRepository;

        public ClienteController(ClienteRepository clienteRepository, ClienteExternoRepository clienteExternoRepository )
        {
            _clienteRepository = clienteRepository;
            this._clienteExternoRepository = clienteExternoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> Get()
        {
            List<Cliente> lCliente = new();
            return await _clienteRepository.ObtenerTodos();
        }

        [HttpGet("GetClienteExterno")]
        public async Task<ActionResult<List<ClienteExterno>>> GetClienteExterno()
        {
            return await _clienteExternoRepository.ObtenerTodos();
        }

        [HttpGet("GetClienteExternoByCgCli/{cg_cli}")]
        public async Task<ActionResult<ClienteExterno>> GetClienteExternoByCgCli(int cg_cli)
        {
            return await _clienteExternoRepository.Obtener(c => c.CG_CLI == cg_cli.ToString()).FirstOrDefaultAsync();
        }

        [HttpGet("Search/{idExterno}/{descripcion}")]
        public async Task<ActionResult<List<ClienteExterno>>> Search(int idExterno, string descripcion )
        {
            IQueryable<ClienteExterno> query = _clienteExternoRepository.ObtenerTodosQueryable();
            if (idExterno > 0)
            {
                query = query.Where(c => c.CG_CLI == idExterno.ToString());
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
            if (_clienteRepository.ObtenerTodosQueryable().Any())
            {
                lCliente = await _clienteRepository.Obtener(p => p.Id == CG_CLI).ToListAsync();
            }
            if (lCliente == null)
            {
                return NotFound();
            }
            return lCliente;
        }
    }
}