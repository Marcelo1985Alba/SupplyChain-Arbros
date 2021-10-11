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
                    lContiene = await _context.Pedidos.Where(p => p.DES_CLI.Contains(Cliente) && p.AVISO == "ALTA DE PRODUCTO FABRICADO").OrderByDescending(s => s.PEDIDO).Take(Busqueda).ToListAsync();
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
                    lContiene = await _context.Pedidos.Where(p => p.PEDIDO.ToString().Contains(Pedido) && p.DES_CLI.Contains(Cliente) && p.CG_ART.Contains(Codigo) && p.AVISO == "ALTA DE PRODUCTO FABRICADO").OrderByDescending(s => s.PEDIDO).Take(Busqueda).ToListAsync();
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
                var genera = await _context.Genera.Where(g => g.CAMP3 == "REGSTOCK").FirstOrDefaultAsync();
                stock.REGISTRO = (int?)genera.VALOR1;
                stock.FE_REG = DateTime.Now;
                stock.USUARIO = "USER";

                if (stock.TIPOO == 9 || stock.TIPOO == 10)
                {
                    stock.CG_ORDEN = _context.Prod.Where(p => p.CG_PROD.Trim() == stock.CG_ART.Trim()).FirstOrDefault().CG_ORDEN;
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
                if (RegistroExists(stock.REGISTRO))
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

        [HttpPost("PostList")]
        public async Task<ActionResult<List<Pedidos>>> PostListStock([FromBody] List<Pedidos> lstock)
        {
            int vale = lstock[0].VALE;
            bool liberaVale = false;
            if (lstock.Count > 0 && !lstock.Any(s=> s.REGISTRO > 0))
            {
                await generaController.ReservaByCampo("VALE");
                var generaVale = await _context.Genera.Where(g => g.CAMP3 == "VALE").FirstOrDefaultAsync();
                vale = (int)generaVale.VALOR1;
                liberaVale = true;
            }
            
            foreach (var stock in lstock)
            {
                try
                {
                    if (stock.REGISTRO < 0) 
                        await AddDb(vale, stock);
                    else
                        await ActualizaDb(stock);

                    if (stock.TIPOO == 5)
                    {
                        stock.Proveedor = _context.Proveedores.Where(p => p.CG_PROVE == stock.CG_PROVE).FirstOrDefault();
                    }

                }
                catch (DbUpdateException ex)
                {
                    await generaController.LiberaByCampo("VALE");
                    await generaController.LiberaByCampo("REGSTOCK");
                    if (RegistroExists(stock.REGISTRO))
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
                    await generaController.LiberaByCampo("VALE");
                    await generaController.LiberaByCampo("REGSTOCK");
                    return BadRequest(e);
                }

            }

            if(liberaVale)
                await generaController.LiberaByCampo("VALE");


            return Ok(lstock);
        }

        private async Task AddDb(int vale, Pedidos stock)
        {
            

            stock.VALE = vale;//el vale se analiza
            stock.CG_CIA = 1;
            //RESERVA REGISTRO: El vale hay que hacerlo del lado del cliente porque debe reservar un solo vale
            //y aqui se ejecuta por item.
            await generaController.ReservaByCampo("REGSTOCK");
            var genera = await _context.Genera.Where(g => g.CAMP3 == "REGSTOCK").FirstOrDefaultAsync();
            stock.REGISTRO = (int?)genera.VALOR1;
            stock.FE_REG = DateTime.Now;
            stock.USUARIO = HttpContext.User.Identity.Name ?? "USER";

            if (stock.TIPOO == 9 || stock.TIPOO == 10)
            {
                stock.CG_ORDEN = _context.Prod.Where(p => p.CG_PROD.Trim() == stock.CG_ART.Trim()).FirstOrDefault().CG_ORDEN;
                stock.STOCK = -stock.STOCK;
            }

            if (stock.TIPOO == 5)
                stock.CUIT = stock.Proveedor?.CUIT.Trim();


            stock.Cliente = null;
            stock.Proveedor = null;
            stock.ResumenStock = null;

            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context.Pedidos.Add(stock);
            await _context.SaveChangesAsync();


            await CerrarOC(stock);
            await FirmeOF(stock);
            await generaController.LiberaByCampo("REGSTOCK");


            if (stock.TIPOO == 9)
            {
                await generaController.ReservaByCampo("REGSTOCK");
                genera = await _context.Genera.Where(g => g.CAMP3 == "REGSTOCK").FirstOrDefaultAsync();
                stock.REGISTRO = (int?)genera.VALOR1;
                stock.USUARIO = "USER";
                stock.CG_CIA = 1;
                stock.STOCK = -stock.STOCK;
                stock.CG_DEP = stock.CG_DEP_ALT;
                _context.Pedidos.Add(stock);
                await _context.SaveChangesAsync();
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
            if (registro != stock.REGISTRO)
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

        private async Task ActualizaDb(Pedidos stock)
        {
            if (stock.TIPOO == 9 || stock.TIPOO == 10)
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
            return _context.Pedidos.Any(e => e.REGISTRO == registro);
        }

        
    }
}