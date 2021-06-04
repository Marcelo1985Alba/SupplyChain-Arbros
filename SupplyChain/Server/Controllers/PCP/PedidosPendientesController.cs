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
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosPendientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidosPendientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PedidosPendientes
        [HttpGet]
        public async Task<IEnumerable<ModeloPedidosPendientes>> GetAsync()
        {
            try
            {
                String xSQLSelect = "EXEC NET_PCP_PEDIDOS";
                var xLista = await _context.ModeloPedidosPendientes.FromSqlRaw(xSQLSelect).ToListAsync();

                return xLista;
            }
            catch (Exception ex)
            {
                return new List<ModeloPedidosPendientes>();
            }
        }
    }
}
