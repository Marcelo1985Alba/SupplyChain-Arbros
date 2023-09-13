using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DireccionesEntregaController : ControllerBase
    {
        private readonly int cg_cia_usuario = 1; /*CAMBIAR POR LA DEL USUARIO*/
        private readonly vDireccionesEntregaRepository _vDireccionesEntregaRepository;

        public DireccionesEntregaController(vDireccionesEntregaRepository vDireccionesEntregaRepository)
        {
            _vDireccionesEntregaRepository = vDireccionesEntregaRepository;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<vDireccionesEntrega>>> GetCompras()
        {
            try
            {
                return await _vDireccionesEntregaRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> GetCompra(int id)
        {
            var compra = await _vDireccionesEntregaRepository.ObtenerPorId(id);

            if (compra == null)
            {
                return NotFound();
            }

            return Ok(compra);
        }

        // GET: api/Compras/GetCompraByNumero/5
        [HttpGet("GetByNumeroCliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<vDireccionesEntrega>>> GetByNumeroCliente(int idCliente)
        {
            try
            {
               return Ok(await _vDireccionesEntregaRepository
                   .Obtener(c=> c.ID_CLIENTE == idCliente.ToString()).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
    }
}
