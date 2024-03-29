﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared.Models;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StockController : Controller
    {
        private int cg_cia_usuario = 1; /*CAMBIAR POR LA DEL USUARIO*/

        private readonly AppDbContext _context;
        private readonly PedidosRepository _pedidosRepository;
        private readonly ProgramaRepository _programaRepository;

        public StockController(AppDbContext context, PedidosRepository pedidosRepository, ProgramaRepository programaRepository)
        {
            _context = context;
            this._pedidosRepository = pedidosRepository;
            this._programaRepository = programaRepository;
        }

        [HttpGet("GetMaxVale")]
        public async Task<IActionResult> GetMaxVale()
        {
            int numero = await _pedidosRepository.MaxNumeroVale(cg_cia_usuario);

            return Json(numero);
        }

        // GET:   
        [HttpGet("GetValesByTipo/{tipoo}/{cantRegistros:int}")]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetValesByTipo(int tipoo, int cantRegistros)
        {
            List<Pedidos> lStock = await _pedidosRepository
                .Obtener(p => p.TIPOO == tipoo && p.VOUCHER == 0 && p.CG_CIA == cg_cia_usuario, cantRegistros,
                         s => s.OrderByDescending(p=> p.VALE), true)
                .ToListAsync();

            if (lStock == null)
            {
                return NotFound();
            }


            return lStock;
        }


        // GET:   
        [HttpGet("GetRemitos/{tipoFiltro}")]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetRemitos(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            List<Pedidos> lStock = await _pedidosRepository.GetRemitos(tipoFiltro, cg_cia_usuario);

            return lStock;
        }


        // GET:   
        [HttpGet("GetRemito/{remito}")]
        public async Task<ActionResult<PedidoEncabezado>> GetRemito(string remito)
        {
            var remLista = await _pedidosRepository.GetRemito(remito);

            return remLista;
        }


        // GET: api/Stock/{vale}
        [HttpGet("ByNumeroVale/{vale}")]
        public async Task<ActionResult<List<Pedidos>>> AbriVale(int vale)
        {
            try
            {
                List<Pedidos> lStock = await _pedidosRepository.ObtenerByNumeroVale(vale, cg_cia_usuario);
                return lStock == null ? NotFound() : lStock;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        // GET: api/Stock/GetByResumenStock
        [HttpGet("GetByResumenStock")]
        public async Task<ActionResult<Pedidos>> GetByResumenStock([FromQuery] ResumenStock resumenStock)
        {
            try
            {
                resumenStock.DESPACHO = resumenStock.DESPACHO == null ? "" : resumenStock.DESPACHO;
                resumenStock.LOTE = resumenStock.LOTE == null ? "" : resumenStock.LOTE;
                resumenStock.SERIE = resumenStock.SERIE == null ? "" : resumenStock.SERIE;

                return await _context.Pedidos.Where(r =>
                r.CG_DEP == resumenStock.CG_DEP
                && r.CG_ART.ToUpper() == resumenStock.CG_ART.ToUpper()
                && r.LOTE.ToUpper() == resumenStock.LOTE.ToUpper()
                && r.DESPACHO.ToUpper() == resumenStock.DESPACHO.ToUpper()
                && r.SERIE.ToUpper() == resumenStock.SERIE.ToUpper()
                ).Take(1).OrderByDescending(r => r.Id).FirstAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }


        [HttpGet("GetListaByPedidos")]
        public async Task<ActionResult<PedidoEncabezado>> GetListaByPedidos([FromQuery] List<int> pedidoIds)
        {
            try
            {
                return await _pedidosRepository.GetListaByPedidos(pedidoIds);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Stock/AbriValeByOCParaDevol/{oc}
        [HttpGet("AbriValeByOCParaDevol/{oc}")]
        public async Task<ActionResult<List<Pedidos>>> AbriValeByOCParaDevol(int oc)
        {
            List<Pedidos> lStock = new List<Pedidos>();
            if (_context.Pedidos.Any())
            {
                lStock = await _context.Pedidos.Where(p => p.OCOMPRA == oc && p.CG_CIA == cg_cia_usuario
                && p.TIPOO == 5).ToListAsync();
            }

            if (lStock == null)
            {
                return NotFound();
            }

            //await lStock.ForEachAsync(async s => 
            //{
            //    s.ResumenStock = await _context.ResumenStock.Where(r => r.CG_DEP == s.CG_DEP
            //        && r.CG_ART.ToUpper() == s.CG_ART.ToUpper()
            //        && r.LOTE.ToUpper() == s.LOTE.ToUpper()
            //        && r.DESPACHO.ToUpper() == s.DESPACHO.ToUpper()
            //        && r.SERIE.ToUpper() == s.SERIE.ToUpper()).FirstAsync();
            //});



            return lStock;
        }

        [HttpGet("GetPedidoEncabezadoById/{id}")]
        public async Task<PedidoEncabezado> GetPedidoEncabezadoById(int id)
        {
            return await _pedidosRepository.ObtenerPedidosEncabezado(id);
        }

        // PUT: api/Stock/PutStock/123729
        [HttpPut("PutStock/{registro}")]
        public async Task<ActionResult<Pedidos>> PutStock(decimal registro, Pedidos stock)
        {
            stock.USUARIO = User.Identity.Name;
            stock.CG_CIA = 1;
            if (registro != stock.Id)
            {
                return BadRequest("Registro Incorrecto");
            }

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegistroExists(registro))
                {
                    return NotFound();
                }
                else
                {
                    BadRequest();
                }
            }

            return Ok(stock);
        }

        //POST: api/Stock
        //To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Pedidos>> PostStock([FromBody] Pedidos stock)
        {
            stock.Id = null;
            stock.USUARIO = HttpContext.User.Identity.Name;
            stock.CG_CIA = 1;
            
            if (stock.TIPOO == 1 || stock.TIPOO == 9) 
                stock.STOCK = -stock.STOCK;

            if (stock.TIPOO == 5)
            {
                stock.CUIT = stock.Proveedor?.CUIT;
            }

            stock.Cliente = null;
            stock.Proveedor = null;


            _context.Pedidos.Add(stock);

            try
            {
                await _context.SaveChangesAsync();

                if (stock.CERRAROC)
                {
                    var compra = await _context.Compras
                        .Where(c => c.CG_MAT == stock.CG_ART && c.NUMERO == stock.OCOMPRA).FirstOrDefaultAsync();

                    compra.FE_CIERRE = DateTime.Now;
                    _context.Entry(compra).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                if (RegistroExists(stock.Id))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest(ex);
                }
            }

            if (stock.TIPOO == 6) //devol a prove: cargo los datos de resumen stock para el item para luego verificar si tiene stock cuando se vuelva a editar
            {
                stock.ResumenStock = await _context.vResumenStock.Where(r => r.CG_DEP == stock.CG_DEP
                && r.CG_ART.ToUpper() == stock.CG_ART.ToUpper()
                && r.LOTE.ToUpper() == stock.LOTE.ToUpper()
                && r.DESPACHO.ToUpper() == stock.DESPACHO.ToUpper()
                && r.SERIE.ToUpper() == stock.SERIE.ToUpper()).FirstAsync();
            }

            //MOVIM ENTRE DEP: GENERAR SEGUNDO REGISTROS: 
            if (stock?.TIPOO == 9)
            {
                stock.Id = null;
                stock.USUARIO = "USER";
                stock.CG_CIA = 1;
                stock.STOCK = -stock.STOCK;
                stock.CG_DEP = stock.CG_DEP_ALT;

                _context.Pedidos.Add(stock);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    if (RegistroExists(stock.Id))
                    {
                        return Conflict();
                    }
                    else
                    {
                        return BadRequest(ex);
                    }
                }

                if (stock.TIPOO == 6) //devol a prove: cargo los datos de resumen stock para el item para luego verificar si tiene stock cuando se vuelva a editar
                {
                    stock.ResumenStock = await _context.vResumenStock.Where(r => r.CG_DEP == stock.CG_DEP
                    && r.CG_ART.ToUpper() == stock.CG_ART.ToUpper()
                    && r.LOTE.ToUpper() == stock.LOTE.ToUpper()
                    && r.DESPACHO.ToUpper() == stock.DESPACHO.ToUpper()
                    && r.SERIE.ToUpper() == stock.SERIE.ToUpper()).FirstAsync();
                }

            }

            return Ok(stock);
        }


        [HttpGet("MovimientosStock")]
        public async Task<List<MovimientoStockSP>> MovimientosStock([FromQuery]FilterMovimientosStock filter)
        {
            try
            {
                var query = "Exec NET_Listado_Movimientos_WEB" +
                        $" @FechaDesde='{filter.Desde}', @FechaHasta ='{filter.Hasta}', @Tipoo = {filter.Tipoo}," +
                        $" @Deposito = {filter.Deposito}";

                var list = await _context.MovimientosStock.FromSqlRaw(query).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                return new List<MovimientoStockSP>();
            }
        }

        [HttpGet("StockInventario")]
        public async Task<ActionResult<List<StockSP>>> Stocks([FromQuery] FilterMovimientosStock filter)
        {
            try
            {
                var query = "Exec NET_Listado_Stock_WEB" +
                        $" @FechaHasta ='{filter.Hasta}', @Deposito = {filter.Deposito}";

                var list = await _context.StocksSP.FromSqlRaw(query).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("TieneRemito/{pedido}")]
        public async Task<ActionResult<bool>> Stocks(int pedido)
        {
            try
            {
                //puede tener alta u orden de armado o remito
                var tienePedido = await _context.Pedidos
                    .AnyAsync(p => (p.TIPOO == 1 && p.PEDIDO == pedido && p.REMITO != "") ||
                                   (p.TIPOO == 28 && p.PEDIDO == pedido) ||
                                    (p.TIPOO == 28 && p.PEDIDO == pedido));
                return tienePedido;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET:   
        [HttpGet("GetPendienteAprobacion")]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetPendienteAprobacion()
        {
            List<Pedidos> lStock = await _pedidosRepository
                .Obtener(p => p.TIPOO == 5 && p.VOUCHER == 0 && p.CG_DEP == 3)
                .ToListAsync();

            if (lStock == null)
            {
                return NotFound();
            }


            return lStock;
        }

        // GET:   
        [HttpGet("GetControlCalidad")]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetControlCalidad()
        {
            try
            {
                string xSQLCommandString = "select * from pedidos as pe where tipoo = 5 and voucher = 0 and cg_dep = 3 and CG_TIRE = 0 AND NOT EXISTS " +
                "(SELECT * FROM PEDIDOS AS P WHERE P.CG_ART = pe.CG_ART and p.DESPACHO = pe.DESPACHO and p.TIPOO != 5) order by FE_MOV desc";

                var xLista = await _context.Pedidos.FromSqlRaw(xSQLCommandString).ToListAsync();

                return xLista;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: api/Stock/GetSegundaGrilla
        [HttpGet("GetSegundaGrilla")]
        public async Task<List<vControlCalidadPendientes>> GetSegundaGrilla()
        {
            try
            {
                string xSQLCommandString = "SELECT P.VALE, P.REGISTRO, P.DESPACHO, P.CG_ART AS CG_PROD, P.DES_ART AS DES_PROD, P.CG_DEP, PR.CG_LINEA," +
                "PMP.DESCAL, PMP.CARCAL, PMP.UNIDADM, PMP.TOLE1, PMP.TOLE2, PMP.AVISO, P.CG_PROVE, P.REMITO, P.OCOMPRA " +
                "FROM   dbo.Pedidos AS P INNER JOIN " +
                "dbo.Prod AS PR ON P.CG_ART = PR.CG_PROD INNER JOIN " +
                "dbo.ProcalMP AS PMP ON PR.CG_LINEA = PMP.CG_LINEA " +
                "WHERE (P.CG_DEP = 3)";


                var xLista = await _context.vcontrolCalidadPendientes.FromSqlRaw(xSQLCommandString).ToListAsync();

                return xLista;
            }
            catch (Exception ex)
            {
                return new List<vControlCalidadPendientes>();
            }
        }

        ////GET:
        //[HttpGet("GetProcalsMP")]
        //public async Task<ActionResult<IEnumerable<ProcalsMP>>> GetProcalsMP()
        //{
        //    List<ProcalsMP> lStock = await _pedidosRepository
        //         .Obtener(v => v.TIPOO == 5 && v.VOUCHER == 0 && v.CG_DEP == 3)
        //         .ToListAsync() ;
        //    if (lStock == null)
        //    {
        //        return NotFound();
        //    }
        //    return lStock;
        //}
        private bool RegistroExists(decimal? registro)
        {
            return _context.Pedidos.Any(e => e.Id == registro);
        }

        // GET: api/Stock/GetPlaneadas
        [HttpGet("GetPlaneadas")]
        private async Task<decimal> GetPlaneadas(string prod)
        {
            decimal toRet = 0;
            List<Programa> programas = await _programaRepository
                .Obtener(p => p.CG_PROD == prod && (p.CG_ESTADO == 0 || p.CG_ESTADO == 1)).ToListAsync();
            if (programas == null)
                return -1;
            foreach(Programa programa in programas)
                toRet += programa.CANTFAB;
            return toRet;
        }
    }
}
