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
using SupplyChain.Shared.Models;
using SupplyChain.Server.Controllers;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoConformidadesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GeneraController generaController;

        public NoConformidadesController(AppDbContext context, GeneraController generaController)
        {
            _context = context;
            this.generaController = generaController;
        }

        /*
        public async Task<ActionResult<IEnumerable<NoConformidades>>> GetPrev()
        {
            try
            {
                return await _context.NoConformidades.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        */
        [HttpGet]
        public IEnumerable<NoConformidadesQuery> Get()
        {
            string xSQL = string.Format("SELECT Cg_NoConf, noconfor.Cg_TipoNc, tiposnoconf.des_tiponc, " +
                "tiposnoconf.origen, Orden, noconfor.Observaciones, Fe_Ocurrencia, " +
                "Fe_Aprobacion, Aprob, Cg_Cli, Cg_Prod, Cg_Orden, Lote, Serie, Despacho, Cg_Ordf, " +
                "Pedido, noconfor.Cg_Cia, Usuario,CG_PROVE, OCOMPRA, CANT, NOCONF, FE_EMIT, FE_PREV, " +
                "FE_SOLUC, DES_CLI, DES_PROVE, Comentarios, fe_implemen, fe_cierre " +
                "from noconfor, tiposnoconf " +
                "where noconfor.cg_tiponc = tiposnoconf.cg_tiponc and orden = 1 " +
                "ORDER BY CG_NOCONF DESC");
            return _context.NoConformidadesQuery.FromSqlRaw(xSQL).ToList();
        }
        
        // GET: api/NoConformidades/GetDespachosByOF/cg_ordf
        [HttpGet("GetDespachosByOF/{cg_ordf}")]
        public async Task<ActionResult<IEnumerable<ModeloPedidosDespacho>>> GetDespachosByOF(decimal cg_ordf)
        {
            /*
            return await _context.Pedidos.Where(p => p.DESPACHO != ""
                    && p.CG_ORDF == cg_ordf).Distinct().ToListAsync();
            */

            string xSQL = "SELECT DISTINCT A.DESPACHO FROM Pedidos A " +
                              "WHERE A.DESPACHO <> '' AND A.CG_ORDF = " + cg_ordf;
                return await _context.PedidosDespacho.FromSqlRaw(xSQL).ToListAsync();
        }

        // GET: api/NoConformidades/GetLotesByOF/cg_ordf
        [HttpGet("GetLotesByOF/{cg_ordf}")]
        public async Task<ActionResult<IEnumerable<ModeloPedidosLote>>> GetLotesByOF(decimal cg_ordf)
        {
            string xSQL = "SELECT DISTINCT A.LOTE FROM Pedidos A " +
                              "WHERE A.LOTE <> '' AND A.CG_ORDF = " + cg_ordf;
            return await _context.PedidosLote.FromSqlRaw(xSQL).ToListAsync();
        }

        // GET: api/NoConformidades/GetDespachosByPedido/pedido
        [HttpGet("GetDespachosByPedido/{pedido}")]
        public async Task<ActionResult<IEnumerable<ModeloPedidosDespacho>>> GetDespachosByPedido(int pedido)
        {
            string xSQL = "SELECT DISTINCT A.DESPACHO FROM Pedidos A " +
                              "WHERE A.DESPACHO <> '' AND A.pedido = " + pedido;
            return await _context.PedidosDespacho.FromSqlRaw(xSQL).ToListAsync();
        }

        // GET: api/NoConformidades/GetLotesByPedido/pedido
        [HttpGet("GetLotesByPedido/{pedido}")]
        public async Task<ActionResult<IEnumerable<ModeloPedidosLote>>> GetLotesByPedido(int pedido)
        {
            string xSQL = "SELECT DISTINCT A.LOTE FROM Pedidos A " +
                              "WHERE A.LOTE <> '' AND A.PEDIDO = " + pedido;
            return _context.PedidosLote.FromSqlRaw(xSQL).ToList();
        }

        // GET: api/NoConformidades/GetOrdenCompra/ocompra
        [HttpGet("GetOrdenCompra/{ocompra}")]
        public async Task<ActionResult<IEnumerable<Compra>>> GetOrdenCompra(int ocompra)
        {
            string xSQL = "SELECT * FROM Compras A " +
                              "WHERE A.NUMERO = " + ocompra;
            return _context.ComprasDbSet.FromSqlRaw(xSQL).ToList();
        }

        // GET: api/NoConformidades/GetDespachosByOC/ocompra
        [HttpGet("GetDespachosByOC/{ocompra}")]
        public async Task<ActionResult<IEnumerable<ModeloPedidosDespacho>>> GetDespachosByOC(int ocompra)
        {
            string xSQL = "SELECT DISTINCT A.DESPACHO FROM Pedidos A " +
                              "WHERE A.DESPACHO <> '' AND CG_TIRE = 2 AND A.OCOMPRA = " + ocompra;
            return _context.PedidosDespacho.FromSqlRaw(xSQL).ToList();
        }

        // GET: api/NoConformidades/GetLotesByOC/ocompra
        [HttpGet("GetLotesByOC/{ocompra}")]
        public async Task<ActionResult<IEnumerable<ModeloPedidosLote>>> GetLotesByOC(int ocompra)
        {
            string xSQL = "SELECT CAST(A.CG_ORDING AS VARCHAR(10)) AS LOTE FROM Pedidos A " +
                              "WHERE A.CG_ORDING > 0 AND A.OCOMPRA = " + ocompra;
            return _context.PedidosLote.FromSqlRaw(xSQL).ToList();
        }



        //POST: api/NoConf
        [HttpPost]
        public async Task<ActionResult<NoConformidades>> PostStock([FromBody] NoConformidades NoConf)
        {

            try
            {
                //RESERVA REGISTRO: El vale hay que hacerlo del lado del cliente porque debe reservar un solo vale
                //y aqui se ejecuta por item.
                await generaController.ReservaByCampo("NOCONF");
                var genera = _context.Genera.Where(g => g.CAMP3 == "NOCONF").FirstOrDefault();
                NoConf.Cg_NoConf = (int)genera.VALOR1;


                _context.NoConformidades.Add(NoConf);
                await _context.SaveChangesAsync();
                await generaController.LiberaByCampo("NOCONF");
            }
            //            catch (DbUpdateException ex)
            //            {
            //                await generaController.LiberaByCampo("NOCONF");
            //              if (RegistroExists(NoConf.Cg_NoConf))
            //                {
            //                    return Conflict();
            //                }
            //                else
            //                {
            //                  return BadRequest(ex);
            //                }
            //            }
            catch (Exception e)
            {
                await generaController.LiberaByCampo("NOCONF");
                return BadRequest(e);
            }
            return Ok(NoConf);
        }

        //POST: api/NoConformidades/123729
        [HttpPut("{registro}")]
        public async Task<ActionResult<NoConformidades>> PutStock(int Cg_NoConf, NoConformidades NoConf)
        //public async Task<ActionResult<Pedidos>> PutStock(decimal registro, Pedidos stock)
        {
            if (NoConf.Cg_NoConf == 0)
            {
                return BadRequest("Registro Incorrecto");
            }

            _context.Entry(NoConf).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                if (!RegistroExists(Cg_NoConf))
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

            return Ok(NoConf);
        }

        private bool RegistroExists(decimal? Cg_NoConf)
        {
            return _context.NoConformidades.Any(e => e.Cg_NoConf == Cg_NoConf);
        }

        // GET: api/NoConformidades/GetAccionesByEvento/cg_noconf
        [HttpGet("GetAccionesByEvento/{cg_noconf}")]
        public async Task<ActionResult<IEnumerable<NoConformidadesAcciones>>> GetAccionesByEvento(int cg_noconf)
        {
            string xSQL = "SELECT a.Cg_NoConfAcc, a.Cg_NoConf, a.Orden, b.Texto as DesOrden, a.Observaciones, a.Fe_ocurrencia, a.Usuario " +
                "FROM NoConfor_Acciones as a " +
                "left join NoConfor_ListaAcciones as b on a.Orden = b.Tipoaccion " +
                " WHERE a.Cg_NoConf = " + cg_noconf;
            return await _context.NoConformidadesAcciones.FromSqlRaw(xSQL).ToListAsync();
        }


        [HttpGet("Eventos")]
        public async Task<ActionResult<List<vEstadEventos>>> GetEventos()
        {
            try
            {
                return await _context.vEstadEventos.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}