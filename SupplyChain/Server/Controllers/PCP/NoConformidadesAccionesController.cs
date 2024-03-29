﻿using System;
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
    public class NoConformidadesAccionesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GeneraController generaController;

        public NoConformidadesAccionesController(AppDbContext context, GeneraController generaController)
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
        [HttpGet]
        public IEnumerable<NoConformidadesQuery> Get()
        {
            string xSQL = string.Format("SELECT Cg_NoConf, noconfor.Cg_TipoNc, tiposnoconf.des_tiponc, " +
                "tiposnoconf.origen, Orden, noconfor.Observaciones, Fe_Ocurrencia, " +
                "Fe_Aprobacion, Aprob, Cg_Cli, Cg_Prod, Cg_Orden, Lote, Serie, Despacho, Cg_Ordf, " +
                "Pedido, noconfor.Cg_Cia, Usuario,CG_PROVE, OCOMPRA, CANT, NOCONF, FE_EMIT, FE_PREV, " +
                "FE_SOLUC, DES_CLI, DES_PROVE, Comentarios " +
                "from noconfor, tiposnoconf " +
                "where noconfor.cg_tiponc = tiposnoconf.cg_tiponc and orden = 1 " +
                "ORDER BY CG_NOCONF DESC");
            return _context.NoConformidadesQuery.FromSqlRaw(xSQL).ToList();
        }
        */


        // GET: api/NoConformidadesAcciones/GetAccionxCgNoConf/1/2
        [HttpGet("GetAccionxCgNoConf/{Cg_NoConf}/{Orden}")]
        public async Task<ActionResult<IEnumerable<NoConformidadesAcciones>>> GetAccionxCgNoConf(int Cg_NoConf, int Orden)

        {
            try
            {
                string xSQL = "SELECT a.Cg_NoConfAcc, a.Cg_NoConf, a.Orden, b.Texto as DesOrden, a.Observaciones, a.Fe_ocurrencia, a.Usuario " +
                    "FROM NoConfor_Acciones as a " +
                    "left join NoConfor_ListaAcciones as b on a.Orden = b.Tipoaccion " +
                    " WHERE a.Cg_NoConf = " + Cg_NoConf + " and a.Orden = " + Orden;
                return await _context.NoConformidadesAcciones.FromSqlRaw(xSQL).ToListAsync();
            }
            catch
            {
                return new List<NoConformidadesAcciones>();
            }
        }

        // DELETE: api/NoConformidadesAcciones/1
        [HttpDelete("{Cg_NoConfAcc}")]
        public async Task<ActionResult<NoConformidadesAcciones>> DeleteAccion(int Cg_NoConfAcc)
        {
            var NoConfAccion = await _context.NoConformidadesAcciones.FindAsync(Cg_NoConfAcc);
            if (NoConfAccion == null)
            {
                return NotFound();
            }

            _context.NoConformidadesAcciones.Remove(NoConfAccion);
            await _context.SaveChangesAsync();

            return NoConfAccion;
        }

        //POST: api/NoConfAcciones
        [HttpPost]
        public async Task<ActionResult<NoConformidadesAcciones>> PostStock([FromBody] NoConformidadesAcciones NoConfAcciones)
        {

            try
            {
                _context.NoConformidadesAcciones.Add(NoConfAcciones);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            return Ok(NoConfAcciones);
        }
        /*

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

        private bool RegistroExists(decimal? registro)
        {
            return _context.Pedidos.Any(e => e.REGISTRO == registro);
        }
        */


        // GET: api/NoConformidadesAcciones/GetListaAcciones/
        [HttpGet("GetListaAcciones/")]
        public async Task<ActionResult<IEnumerable<NoConformidadesListaAcciones>>> GetListaAcciones(int ocompra)
        {
            string xSQL = "SELECT * FROM NoConfor_ListaAcciones ";
            return _context.NoConformidadesListaAcciones.FromSqlRaw(xSQL).ToList();
        }

    }
}