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
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosPendientesController : ControllerBase
    {
        private readonly PedCliRepository _pedCliRepository;

        public PedidosPendientesController(PedCliRepository pedCliRepository)
        {
            this._pedCliRepository = pedCliRepository;
        }

        // GET: api/PedidosPendientes
        [HttpGet]
        public async Task<List<ModeloPedidosPendientes>> GetAsync()
        {
            try
            {
                var xLista = await _pedCliRepository.ObtenerPedidosPedientes();

                return xLista.ToList();
            }
            catch (Exception ex)
                {
                return new List<ModeloPedidosPendientes>();
            }
        }

        [HttpPut("{pedido}")]
        public async Task<ActionResult<ModeloPedidosPendientes>> Put(int pedido, ModeloPedidosPendientes modeloPedidosPendientes)
        {
            if (pedido != modeloPedidosPendientes.PEDIDO)
            {
                return NotFound();
            }

            var pedcli = await _pedCliRepository.Obtener(p => p.PEDIDO == modeloPedidosPendientes.PEDIDO).FirstOrDefaultAsync();
            pedcli.CAMPOCOM2 = modeloPedidosPendientes.CAMPOCOM2;//resorte
            pedcli.ENTRPREV = modeloPedidosPendientes.ENTRPREV;

            try
            {
                await _pedCliRepository.Actualizar(pedcli);

                return Ok(modeloPedidosPendientes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
