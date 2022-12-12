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
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Controllers;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GeneraController generaController;

        public PedidosController(AppDbContext context, GeneraController generaController)
        {
            _context = context;
            this.generaController = generaController;
        }

        [HttpGet]
        public async Task<IEnumerable<Pedidos>> Get(string PEDIDO)
        {
            string xSQL = string.Format("" +
                "SELECT Pedidos.* " +
                "FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) " +
                "where(pedidos.FLAG = 0 AND Programa.CG_ESTADO = 3 AND Pedidos.CG_ORDF != 0 AND(Pedidos.TIPOO = 1)) " +
                "UNION SELECT Pedidos.* " +
                "FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) " +
                "where Pedcli.PEDIDO NOT IN(select PEDIDO from Pedidos where TIPOO = 1) AND Programa.CG_ESTADO = 3  AND Pedcli.CANTPED > 0 AND Pedidos.TIPOO != 28");
            return await _context.Pedidos.FromSqlRaw(xSQL).ToListAsync();
        }

        // GET: api/Pedidos/BuscarPorPedido/{Pedido}
        [HttpGet("BuscarPorPedido/{Pedido}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarPorPedido(string Pedido)
        {
            List<Pedidos> lPedidos = new();
            if (_context.Pedidos.Any())
            {
                lPedidos = await _context.Pedidos.Where(p => p.PEDIDO.ToString() == Pedido).ToListAsync();
            }
            return
                lPedidos == null ? NotFound() : lPedidos;
        }

        // GET: api/Pedidos/BuscarPorCliente/{Cliente}
        [HttpGet("BuscarPorCliente/{Cliente}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarPorCliente(string Cliente)
        {
            List<Pedidos> lPedidos = new();
            if (_context.Pedidos.Any())
            {
                lPedidos = await _context.Pedidos.Where(p => p.DES_CLI == Cliente).ToListAsync();
            }
            return lPedidos == null ? NotFound() : lPedidos;
        }

        // GET: api/Pedidos/BuscarPorCodigo/{Codigo}
        [HttpGet("BuscarPorCodigo/{Codigo}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarPorCodigo(string Codigo)
        {
            List<Pedidos> lPedidos = new();
            if (_context.Pedidos.Any())
            {
                lPedidos = await _context.Pedidos.Where(p => p.CG_ART == Codigo).ToListAsync();
            }
            return lPedidos == null ? NotFound() : lPedidos;
        }

        // GET: api/Pedidos/BuscarPorOF/{OF}
        [HttpGet("BuscarPorOF/{OF}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarPorOF(string OF)
        {
            List<Pedidos> lPedidos = new();
            if (_context.Pedidos.Any())
            {
                lPedidos = await _context.Pedidos.Where(p => p.CG_ORDF.ToString() == OF).ToListAsync();
            }
            if (lPedidos == null)
            {
                return NotFound();
            }
            return lPedidos;
        }

        // GET:
        [HttpGet("BuscarTrazabilidad/{Pedido}/{Cliente}/{Codigo}/{Busqueda}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarTrazabilidad(string Pedido, string Cliente, string Codigo, int Busqueda)
        {
            List<Pedidos> lContiene = new();
            if (Pedido != "Vacio")
            {
                if (_context.Pedidos.Any())
                {
                    lContiene = await _context.Pedidos.Where(p => p.PEDIDO.ToString().Contains(Pedido) 
                    && p.AVISO == "ALTA DE PRODUCTO FABRICADO").OrderByDescending(s => s.PEDIDO).Take(Busqueda).ToListAsync();
                }
                if (lContiene == null)
                {
                    return NotFound();
                }
            }
            else if (Cliente != "Vacio")
            {
                if (_context.Pedidos.Any())
                {
                    lContiene = await _context.Pedidos.Where(p => p.DES_CLI.Contains(Cliente) && p.AVISO == "ALTA DE PRODUCTO FABRICADO")
                        .OrderByDescending(s => s.PEDIDO).Take(Busqueda).ToListAsync();
                }
                if (lContiene == null)
                {
                    return NotFound();
                }
            }
            else if (Codigo != "Vacio")
            {
                if (_context.Pedidos.Any())
                {
                    lContiene = await _context.Pedidos.Where(p => p.CG_ART.Contains(Codigo) && p.AVISO == "ALTA DE PRODUCTO FABRICADO").OrderByDescending(s => s.PEDIDO).Take(Busqueda).ToListAsync();
                }
                if (lContiene == null)
                {
                    return NotFound();
                }
            }
            else
            {
                if (_context.Pedidos.Any())
                {
                    lContiene = await _context.Pedidos.Where(p =>  p.AVISO == "ALTA DE PRODUCTO FABRICADO").OrderByDescending(s => s.PEDIDO).Take(Busqueda).ToListAsync();
                }
                if (lContiene == null)
                {
                    return NotFound();
                }
            }
            return lContiene;
        }

        // GET: api/Pedidos/MostrarTrazabilidad/{Pedido}
        [HttpGet("MostrarTrazabilidad/{Pedido}")]
        public async Task<ActionResult<List<Pedidos>>> MostrarTrazabilidad(string Pedido)
        {
            try
            {
                List<Pedidos> lPedidos = new();
                if (_context.Pedidos.Any())
                {
                    lPedidos = await _context.Pedidos.Where(p => p.PEDIDO.ToString() == Pedido).ToListAsync();
                }
                return lPedidos == null ? NotFound() : lPedidos;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Pedidos/BusquedaParaFE_MOV/{PEDIDO}
        [HttpGet("BusquedaParaFE_MOV/{PEDIDO}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarPorCG_PROD(string PEDIDO)
        {
            List<Pedidos> lpedidos = new();
            if (_context.Pedidos.Any())
            {
                lpedidos = await _context.Pedidos.Where(p => p.PEDIDO.ToString() == PEDIDO && p.CG_ORDEN == 1).ToListAsync();
            }
            if (lpedidos == null)
            {
                return NotFound();
            }
            return lpedidos;
        }

        //POST: api/Stock
        //To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Pedidos>> PostStock([FromBody] Pedidos stock)
        {
            try
            {
                stock.CG_CIA = 1;
                //RESERVA REGISTRO: El vale hay que hacerlo del lado del cliente porque debe reservar un solo vale
                //y aqui se ejecuta por item.
                await generaController.ReservaByCampo("REGSTOCK");
                var genera = await _context.Genera.Where(g => g.Id == "REGSTOCK").FirstOrDefaultAsync();
                stock.Id = (int)genera.VALOR1;
                stock.FE_REG = DateTime.Now;
                stock.USUARIO = "USER";

                if (stock.TIPOO == 9 || stock.TIPOO == 10)
                {
                    stock.CG_ORDEN = _context.Prod.Where(p => p.Id.Trim() == stock.CG_ART.Trim()).FirstOrDefault().CG_ORDEN;
                    stock.STOCK = -stock.STOCK;
                }

                if (stock.TIPOO == 5)
                    stock.CUIT = stock.Proveedor?.CUIT.Trim();

                stock.Cliente = null;
                stock.Proveedor = null;
                _context.Pedidos.Add(stock);
                await _context.SaveChangesAsync();

                await CerrarOC(stock);
                await FirmeOF(stock);
                await generaController.LiberaByCampo("REGSTOCK");
            }
            catch (DbUpdateException ex)
            {
                await generaController.LiberaByCampo("REGSTOCK");
                if (RegistroExists(stock.Id))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest(ex);
                }
            }
            catch(Exception e)
            {
                await generaController.LiberaByCampo("REGSTOCK");
                return BadRequest(e);
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

        [HttpPost("PostList")]
        public async Task<ActionResult<List<Pedidos>>> PostListStock([FromBody] List<Pedidos> lstock)
        {
            int vale = lstock[0].VALE;
            int remito = 0;
            bool liberaVale = false;
            bool liberaRemito = false;
            //vales
            if (lstock.Count > 0 && !lstock.Any(s=> s.Id > 0) && lstock.All(s=> s.TIPOO != 1))
            {
                await generaController.ReservaByCampo("VALE");
                var generaVale = await _context.Genera.Where(g => g.Id == "VALE").FirstOrDefaultAsync();
                vale = (int)generaVale.VALOR1;
                liberaVale = true;
            }

            //remitos
            if (lstock.Any(s => s.TIPOO == 1 && s.Id < 0))
            {
                await generaController.ReservaByCampo("REMITO");
                var generaVale = await _context.Genera.Where(g => g.Id == "REMITO").FirstOrDefaultAsync();
                remito = (int)generaVale.VALOR1;
                liberaRemito = true;

            }

            
            foreach (var stock in lstock)
            {
                try
                {
                    if (liberaRemito)
                    { 
                        var rem = remito.ToString().PadLeft(8, '0');
                        stock.REMITO = $"0001-{rem}";

                        stock.Id ??= 0;
                    }

                    if(stock.TIPOO == 1)
                    {
                        stock.STOCKA = -1;
                        if (stock.STOCK > 0)
                        {
                            stock.STOCK = -stock.STOCK;
                        }
                    }


                    if (stock.Id < 0) 
                        await AddDb(vale, stock);
                    else
                        await ActualizaDb(stock);

                    if (stock.TIPOO == 5)
                    {
                        stock.Proveedor = _context.Proveedores.AsNoTracking()
                            .Where(p => p.Id == stock.CG_PROVE).FirstOrDefault();
                    }

                }
                catch (DbUpdateException ex)
                {
                    if (liberaVale)
                        await generaController.LiberaByCampo("VALE");

                    if (liberaRemito)
                        await generaController.LiberaByCampo("REMITO");

                    if (stock.Id < 0)
                    {
                        await generaController.LiberaByCampo("REGSTOCK");
                    }
                    
                    if (RegistroExists(stock.Id))
                    {
                        return Conflict();
                    }
                    else
                    {
                        return BadRequest(ex);
                    }
                }
                catch (Exception e)
                {
                    if (liberaVale)
                        await generaController.LiberaByCampo("VALE");

                    if (liberaRemito)
                        await generaController.LiberaByCampo("REMITO");

                    await generaController.LiberaByCampo("REGSTOCK");
                    return BadRequest(e);
                }

            }

            
            await FirmeOF(lstock[0]);

            if(liberaVale)
                await generaController.LiberaByCampo("VALE");

            if (liberaRemito)
                await generaController.LiberaByCampo("REMITO");


            return Ok(lstock);
        }

        private async Task AddDb(int vale, Pedidos stock)
        {
            

            stock.VALE = vale;//el vale se analiza
            stock.CG_CIA = 1;
            //RESERVA REGISTRO: El vale hay que hacerlo del lado del cliente porque debe reservar un solo vale
            //y aqui se ejecuta por item.
            await generaController.ReservaByCampo("REGSTOCK");
            var genera = await _context.Genera.Where(g => g.Id == "REGSTOCK").FirstOrDefaultAsync();
            stock.Id = (int)genera.VALOR1;
            stock.FE_REG = DateTime.Now;
            stock.USUARIO = HttpContext.User.Identity.Name ?? "USER";

            if (stock.TIPOO == 9 || stock.TIPOO == 10 || stock.TIPOO == 28)
            {
                stock.CG_ORDEN = _context.Prod.Where(p => p.Id.Trim() == stock.CG_ART.Trim()).FirstOrDefault().CG_ORDEN;
                stock.STOCK = -stock.STOCK;
            }

            if (stock.TIPOO == 5)
                stock.CUIT = stock.Proveedor?.CUIT.Trim();


            stock.Cliente = null;
            stock.Proveedor = null;
            stock.ResumenStock = null;

            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;

            _context.Pedidos.Add(stock);
            await _context.SaveChangesAsync();

            if (stock.TIPOO == 1)
            {
                var update = "UPDATE PEDCLI SET CG_ESTADO = 'C' " +
                    $"WHERE PEDIDO = {stock.PEDIDO} AND CG_ART = '{stock.CG_ART}'";
                _context.Database.ExecuteSqlRaw(update);
            }

            await CerrarOC(stock);

            await generaController.LiberaByCampo("REGSTOCK");


            if (stock.TIPOO == 9)
            {
                await generaController.ReservaByCampo("REGSTOCK");
                genera = await _context.Genera.Where(g => g.Id == "REGSTOCK").FirstOrDefaultAsync();
                stock.Id = (int?)genera.VALOR1;
                stock.USUARIO = "USER";
                stock.CG_CIA = 1;
                stock.STOCK = -stock.STOCK;
                stock.CG_DEP = stock.CG_DEP_ALT;
                _context.Pedidos.Add(stock);
                await _context.SaveChangesAsync();
                await generaController.LiberaByCampo("REGSTOCK");
            }


        }

        private async Task CerrarOC(Pedidos stock)
        {
            if (stock.TIPOO ==5)
            {
                var recibido =_context.Pedidos
                    .Where(p => p.TIPOO == 5 && p.OCOMPRA == stock.OCOMPRA && p.CG_ART == stock.CG_ART)
                    .Sum(p => p.STOCK);

                var cantOC = _context.Compras
                    .Where(c => c.NUMERO == stock.OCOMPRA && c.CG_MAT == stock.CG_ART)
                    .Sum(c => c.SOLICITADO);

                var completado = cantOC - recibido <= 0;

                if (stock.CERRAROC || completado)
                {
                    var compra = await _context.Compras
                        .Where(c => c.CG_MAT == stock.CG_ART && c.NUMERO == stock.OCOMPRA).FirstOrDefaultAsync();

                    compra.FE_CIERRE = DateTime.Now;
                    _context.Entry(compra).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            
        }
        private async Task FirmeOF(Pedidos stock)
        {
            if (stock.TIPOO == 28)
            {
                var programa = await _context.Programa.AsNoTracking()
                               .FirstOrDefaultAsync(c => c.CG_ORDF == stock.CG_ORDF || c.CG_ORDFASOC == stock.CG_ORDF);

                programa.CG_ESTADO = 1;
                programa.CG_ESTADOCARGA = 2;
                programa.FE_FIRME = DateTime.Now;
                _context.Entry(programa).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }


            if (stock.TIPOO == 10)
            {
                var programa = await _context.Programa.AsNoTracking()
                                .Where(c => c.CG_ORDF == stock.CG_ORDF || c.CG_ORDFASOC == stock.CG_ORDF).ToListAsync();

                if (programa.Count > 0 && programa.Any(p => p.CG_ESTADOCARGA != 2))
                {
                    await programa.ForEachAsync(async p =>
                    {
                        p.CG_ESTADO = 1;
                        p.CG_ESTADOCARGA = 2;
                        p.FE_FIRME = DateTime.Now;
                        _context.Entry(p).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    });
                }
            }
            
        }

        // PUT: api/Stock/123729
        [HttpPut("{registro}")]
        public async Task<ActionResult<Pedidos>> PutStock(decimal registro, Pedidos stock)
        {
            stock.USUARIO = "USER";
            stock.CG_CIA = 1;
            if (registro != stock.Id)
            {
                return BadRequest("Registro Incorrecto");
            }


            try
            {
                 await ActualizaDb(stock);
                
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                if (!RegistroExists(registro))
                {
                    return NotFound();
                }
                else
                {
                    BadRequest(dbEx);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(stock);
        }

        [HttpPut("FromPedCli")]
        public async Task<ActionResult<Pedidos>> PutStockFromPedCli(PedCli pedCli)
        {
            List<Pedidos> lStocks = new();
            try
            {
                lStocks = await _context.Pedidos.Where(p => p.PEDIDO == pedCli.PEDIDO && p.TIPOO == 1).ToListAsync();
                foreach (var stock in lStocks)
                {
                    stock.CONFIRMADO = pedCli.CONFIRMADO ? 1: 0;
                    await ActualizaDb(stock);
                }
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                return BadRequest(dbEx);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(lStocks);
        }

        private async Task ActualizaDb(Pedidos stock)
        {
            if (stock.TIPOO == 9 || stock.TIPOO == 10 || stock.TIPOO == 28)
                stock.STOCK = -stock.STOCK;

            _context.Entry(stock).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        [HttpDelete("{vale}")]
        public async Task<IActionResult> DeleteByVale(int vale)
        {
            var pedidos = await _context.Pedidos.Where(p => p.VALE == vale).ToListAsync();
            if (pedidos is null || pedidos.Count == 0)
            {
                return NotFound();
            }

            foreach (var item in pedidos)
            {
                item.STOCK = 0;
                item.PEDIDO = 0;
                item.CG_ORDF = 0;
                item.AVISO = "VALE ANULADO";
                _context.Entry(item).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private bool RegistroExists(decimal? registro)
        {
            return _context.Pedidos.Any(e => e.Id == registro);
        }

        
    }
}