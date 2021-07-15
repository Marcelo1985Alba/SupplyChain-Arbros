using System;
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

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : Controller
    {
        private int cg_cia_usuario = 1; /*CAMBIAR POR LA DEL USUARIO*/

        private readonly AppDbContext _context;
        public StockController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetMaxVale")]
        public async Task<IActionResult> GetMaxVale()
        {
            //var user = User.Identities.ToList().Select(u=> u.Claims);
            int numero = 1;
            if (await _context.Pedidos.CountAsync() > 0)
                numero += await _context.Pedidos
                    .Where(p => p.CG_CIA == cg_cia_usuario).MaxAsync(p => p.VALE);

            return Json(numero);
        }

        // GET:   
        [HttpGet("GetValesByTipo/{tipoo}/{cantRegistros:int}")]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetValesByTipo(int tipoo, int cantRegistros)
        {
            List<Pedidos> lStock = new List<Pedidos>();
            if (_context.Pedidos.Any())
            {
                lStock = await _context.Pedidos
                    //.Include(x=> x.Proveedor)
                    .Where(p => p.TIPOO == tipoo && p.VOUCHER == 0 && p.CG_CIA == cg_cia_usuario)
                    .OrderByDescending(s=> s.VALE)
                    .Take(cantRegistros)
                    .ToListAsync();
            }
            if (lStock == null)
            {
                return NotFound();
            }


            return lStock.OrderByDescending(s => s.VALE).ToList(); ;
        }

        // GET: api/Stock/AbriVale/{vale}
        [HttpGet("AbriVale/{vale}")]
        public async Task<ActionResult<List<Pedidos>>> AbriVale(int vale)
        {
            try
            {
                List<Pedidos> lStock = new List<Pedidos>();
                if (_context.Pedidos.Any())
                {
                    lStock = await _context.Pedidos
                        .Where(p => p.VALE == vale && p.CG_CIA == cg_cia_usuario)
                        .ToListAsync();
                }



                if (lStock == null)
                {
                    return NotFound();
                }

                //SI ES RECEPCION HAYQ EU CALCULAR PENDIENTE DE OC Y OBTENER EL PEDIDO
                if ( lStock.Count > 0 && lStock[0].TIPOO == 5)
                {
                    await lStock.ForEachAsync(async s =>
                    {
                        var solicitado = await _context.Compras.Where(c => c.NUMERO == s.OCOMPRA && c.CG_MAT == s.CG_ART)
                        .SumAsync(c => c.SOLICITADO);
                        var recibido = await _context.Pedidos
                            .Where(p => p.TIPOO == 5 && p.OCOMPRA == s.OCOMPRA && p.CG_ART == s.CG_ART)
                            .SumAsync(p => p.STOCK);

                        s.PENDIENTEOC = solicitado - recibido;


                        s.Proveedor = await _context.Proveedores.Where(p => p.CG_PROVE == s.CG_PROVE).FirstOrDefaultAsync();
                    });

                }

                if (lStock.Count > 0 && lStock[0].TIPOO == 10)
                {
                    await lStock.ForEachAsync(async i =>
                    {
                        i.ResumenStock = await _context.vResumenStock.Where(r =>
                             r.CG_ART.ToUpper() == i.CG_ART.ToUpper()
                             && r.LOTE.ToUpper() == i.LOTE.ToUpper()
                             && r.DESPACHO.ToUpper() == i.DESPACHO.ToUpper()
                             && r.SERIE.ToUpper() == i.SERIE.ToUpper()
                             && r.CG_DEP == i.CG_DEP).FirstOrDefaultAsync();

                        i.PENDIENTEOC = i.ResumenStock.STOCK - Math.Abs((decimal)i.STOCK);
                    });
                            

                }


                return lStock;
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
                ).Take(1).OrderByDescending(r => r.REGISTRO).FirstAsync();
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


        // PUT: api/Stock/PutStock/123729
        [HttpPut("PutStock/{registro}")]
        public async Task<ActionResult<Pedidos>> PutStock(decimal registro, Pedidos stock)
        {
            stock.USUARIO = "USER";
            stock.CG_CIA = 1;
            if (registro != stock.REGISTRO)
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
            stock.REGISTRO = null;
            stock.USUARIO = "USER";
            stock.CG_CIA = 1;
            
            if (stock.TIPOO == 9) 
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
                if (RegistroExists(stock.REGISTRO))
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
                stock.REGISTRO = null;
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
                    if (RegistroExists(stock.REGISTRO))
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

        private bool RegistroExists(decimal? registro)
        {
            return _context.Pedidos.Any(e => e.REGISTRO == registro);
        }
    }
}
