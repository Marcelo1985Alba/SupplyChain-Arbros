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
using SupplyChain.Server.Controllers;
using SupplyChain.Shared.PCP;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrazabilidadsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GeneraController generaController;

        public TrazabilidadsController(AppDbContext context, GeneraController generaController)
        {
            _context = context;
            this.generaController = generaController;
        }

        [HttpGet]
        public IEnumerable<Pedidos> Get(string PEDIDO)
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
            return _context.Pedidos.FromSqlRaw(xSQL).ToList<Pedidos>();
        }

        // GET: api/Pedidos/BuscarPorPedido/{Pedido}
        [HttpGet("BuscarPorPedido/{Pedido}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarPorPedido(string Pedido)
        {
            List<Pedidos> lPedidos = new List<Pedidos>();
            if (_context.Pedidos.Any())
            {
                lPedidos = await _context.Pedidos.Where(p => p.PEDIDO.ToString() == Pedido).ToListAsync();
            }
            if (lPedidos == null)
            {
                return NotFound();
            }
            return lPedidos;
        }

        // GET: api/Pedidos/BuscarPorCliente/{Cliente}
        [HttpGet("BuscarPorCliente/{Cliente}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarPorCliente(string Cliente)
        {
            List<Pedidos> lPedidos = new List<Pedidos>();
            if (_context.Pedidos.Any())
            {
                lPedidos = await _context.Pedidos.Where(p => p.DES_CLI == Cliente).ToListAsync();
            }
            if (lPedidos == null)
            {
                return NotFound();
            }
            return lPedidos;
        }

        // GET: api/Pedidos/BuscarPorCodigo/{Codigo}
        [HttpGet("BuscarPorCodigo/{Codigo}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarPorCodigo(string Codigo)
        {
            List<Pedidos> lPedidos = new List<Pedidos>();
            if (_context.Pedidos.Any())
            {
                lPedidos = await _context.Pedidos.Where(p => p.CG_ART == Codigo).ToListAsync();
            }
            if (lPedidos == null)
            {
                return NotFound();
            }
            return lPedidos;
        }

        // GET: api/Pedidos/BuscarPorOF/{OF}
        [HttpGet("BuscarPorOF/{OF}")]
        public async Task<ActionResult<List<Pedidos>>> BuscarPorOF(string OF)
        {
            List<Pedidos> lPedidos = new List<Pedidos>();
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
            List<Pedidos> lContiene = new List<Pedidos>();
            if (Pedido != "Vacio")
            {
                if (_context.Pedidos.Any())
                {
                    lContiene = await _context.Pedidos.Where(p => p.PEDIDO.ToString().Contains(Pedido) && p.AVISO == "ALTA DE PRODUCTO FABRICADO").OrderByDescending(s => s.PEDIDO).Take(Busqueda).ToListAsync();
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

        // GET: api/vTrazabilidad/MostrarTrazabilidad/{Pedido}
        [HttpGet("MostrarTrazabilidad/{Pedido}")]
        public async Task<ActionResult<List<vTrazabilidad>>> MostrarTrazabilidad(string Pedido)
        {
            try
            {
                List<vTrazabilidad> lPedidos = new();
                if (_context.VTrazabilidads.Any())
                {
                    lPedidos = await _context.VTrazabilidads.Where(p => p.PEDIDO.ToString() == Pedido).OrderBy(t => t.CG_LINEA).ToListAsync();
                }
                if (lPedidos == null)
                {
                    return NotFound();
                }
                return lPedidos;
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
            List<Pedidos> lpedidos = new List<Pedidos>();
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
                    stock.STOCK = -stock.STOCK;

                if (stock.TIPOO == 5)
                    stock.CUIT = stock.Proveedor?.CUIT.Trim();

                stock.Cliente = null;
                stock.Proveedor = null;
                _context.Pedidos.Add(stock);
                await _context.SaveChangesAsync();

                await CerrarOC(stock);
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

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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