using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChain.Shared;

namespace SupplyChain
{
    //[EnableQuery]
    [Route("api/[controller]")]
    [ApiController]
    public class EstadisticaVentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstadisticaVentasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetSyncf")]
        public object GetSyncf()
        {
            IQueryable<EstadVenta> data = _context.EstadVentas.AsQueryable();
            return new { Items = data, Count = data.Count() };
        }


        [HttpGet("GetPresupuestos")]
        public object GetPresupuestos()
        {
            IQueryable<EstadVenta> data = _context.EstadVentas.Where(e => e.Presup > 0).AsQueryable();
            return new { Items = data, Count = data.Count() };
        }


        // GET: api/Indicadores
        [HttpGet]
        public async Task<ActionResult<List<EstadVenta>>> Get()
        {
            try
            {
                List<EstadVenta> Estad;
                return await _context.EstadVentas.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("GetFacturacionAsync")]
        public async Task<ActionResult<List<EstadVenta>>> GetFacturacionAsync()
        {
            try
            {
                List<EstadVenta> Estad;
                Estad = await _context.EstadVentas.Where(e=> e.AnoFactura > 0).ToListAsync();

                return Estad;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("PedidosIngresados")]
        public async Task<ActionResult<List<vEstadPedidosIngresados>>> GetPedidosIngresados()
        {
            try
            {
                return await _context.vEstadPedidosIngresados.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("PedidosAltas")]
        public async Task<ActionResult<List<vEstadPedidosAlta>>> GetPedidosAltas()
        {
            try
            {
                return await _context.vEstadPedidosAltas.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Facturacion")]
        public async Task<ActionResult<List<vEstadFacturacion>>> GetFacturacion()
        {
            try
            {
                return await _context.vEstadFacturaciones.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Compras")]
        public async Task<ActionResult<List<vEstadCompras>>> GetCompras()
        {
            try
            {
                return await _context.vEstadCompras.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}