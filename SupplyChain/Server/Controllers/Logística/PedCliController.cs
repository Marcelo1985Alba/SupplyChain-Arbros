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
using SupplyChain;
using SupplyChain.Server.Repositorios;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedCliController : ControllerBase
    {
        private readonly PedCliRepository _pedCliRepository;

        public PedCliController(PedCliRepository pedCliRpository)
        {
            this._pedCliRepository = pedCliRpository;
        }

        [HttpGet]
        public async Task<IEnumerable<PedCli>> Get()
        {
            return await _pedCliRepository.ObtenerPedCliPedidos();
        }



        [HttpGet("GetPedidos")]
        public async Task<IEnumerable<PedCli>> Gets()
        {
            return await _pedCliRepository.ObtenerTodos();
        }

        [HttpGet("{pedido}")]
        public async Task<IEnumerable<PedCli>> Gets(int pedido)
        {
            return await _pedCliRepository.Obtener(p=> p.PEDIDO == pedido);
        }

        // PUT: api/Servicios/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedCli(int id, PedCli Pedclis)
        {
            if (id != Pedclis.PEDIDO) return BadRequest();


            try
            {
                await _pedCliRepository.Actualizar(Pedclis);
            }
            catch (Exception ex)
            {
                if (!await _pedCliRepository.Existe(id))
                {
                    return NotFound(ex);
                }
                else
                {
                    return BadRequest(ex);
                }
            }

            return NoContent();
        }
        

        // GET: api/pedcli/BuscarPorPedido/{PEDIDO}
        [HttpGet("BuscarPorPedido/{PEDIDO}")]
        public async Task<ActionResult<List<PedCli>>> BuscarPorPedido(string PEDIDO)
        {
            List<PedCli> lpedcli = new();
            lpedcli = (List<PedCli>)await _pedCliRepository.Obtener(p => p.PEDIDO.ToString() == PEDIDO);
            return lpedcli == null ? NotFound() : lpedcli;
        }
    }
}