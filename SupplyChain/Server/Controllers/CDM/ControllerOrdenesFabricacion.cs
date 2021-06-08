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

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesFabricacionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenesFabricacionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModeloOrdenFabricacion>> Get(int id)
        {
            try
            {
                string xSQL = "SELECT A.CG_ORDF, A.FE_ENTREGA, A.CG_PROD, A.DES_PROD, A.CG_FORM, " +
                                                  "(rtrim(ltrim(A.PROCESO))) AS PROCESO, rtrim(ltrim(A.CG_CELDA)) CG_CELDA, CG_ORDFORIG, " +
                                                  "(select max(cg_ordf) from programa where  CG_ORDFASOC = A.CG_ORDFASOC) ULTIMAORDENASOCIADA, A.CG_ORDFASOC, " +
                                                  "A.CANT, A.CG_ESTADOCARGA, A.CANTFAB, convert(numeric(6, 2), (A.CANTFAB * 100 / A.CANT)) AS AVANCE, A.DIASFAB, " +
                                                  "(A.DIASFAB * isnull((Select Top 1 ValorN From Solution Where Campo = 'HORASDIA'), 1)) AS HORASFAB, B.EXIGEOA, A.PEDIDO, " +
                                                  "FECHA_PREVISTA_FABRICACION, " +
                                                  "CASE WHEN A.FECHA_INICIO_REAL_FABRICACION is not null THEN A.FECHA_INICIO_REAL_FABRICACION ELSE GETDATE() END FECHA_INICIO_REAL_FABRICACION, " +
                                                  "CASE WHEN A.FE_CIERRE is not null THEN A.FE_CIERRE ELSE GETDATE() END FE_CIERRE, " +
                                                  "A.CG_OPER, A.DES_OPER " +
                                                  "FROM Prod B, Programa A " +
                                                  "LEFT JOIN ProTab ON ProTab.PROCESO = A.PROCESO " +
                                                  "LEFT JOIN Celdas ON Celdas.CG_CELDA = A.CG_CELDA " +
                                                  "WHERE A.CG_PROD = B.CG_PROD AND A.CG_ORDF = " + id;
                return await _context.OrdenesFabricacion.FromSqlRaw(xSQL).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                BadRequest(ex);
                return new ModeloOrdenFabricacion();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ModeloOrdenFabricacion xItem)
        {
            if (id != xItem.CG_ORDF)
            {
                return BadRequest();
            }

            try
            {
                var programa = await _context.Programa.Where(p => p.CG_ORDF == id).FirstOrDefaultAsync();
                programa.FECHA_PREVISTA_FABRICACION = xItem.FECHA_PREVISTA_FABRICACION;
                programa.FECHA_INICIO_REAL_FABRICACION = xItem.FECHA_INICIO_REAL_FABRICACION;
                programa.CANTFAB = xItem.CANTFAB;
                programa.FE_CIERRE = xItem.FE_CIERRE;
                programa.CG_ORDFORIG = xItem.CG_ORDFORIG;
                programa.CG_CELDA = xItem.CG_CELDA;
                programa.PROCESO = xItem.PROCESO;
                programa.CG_OPER = xItem.CG_OPER;
                programa.DES_OPER = xItem.DES_OPER;



                //string xSQL = string.Format("set dateformat dmy UPDATE Programa SET FECHA_PREVISTA_FABRICACION = '{0}', FECHA_INICIO_REAL_FABRICACION = '{1}', CANTFAB = {2}, FE_CIERRE = '{3}', " +
                //                            "CG_ORDFORIG = {4}, CG_CELDA = '{5}', PROCESO = '{6}', CG_OPER = {7}, DES_OPER = '{8}' WHERE Cg_ordf = {9}",
                //                          xItem.FECHA_PREVISTA_FABRICACION,
                //                          xItem.FECHA_INICIO_REAL_FABRICACION,
                //                          xItem.CANTFAB.ToString().Replace(",", "."),
                //                          xItem.FE_CIERRE,
                //                          xItem.CG_ORDFORIG,
                //                          xItem.CG_CELDA,
                //                          xItem.PROCESO,
                //                          xItem.CG_OPER,
                //                          xItem.DES_OPER,
                //                          xItem.CG_ORDF);

                _context.Attach(programa);

                _context.Entry(programa).Property(p => p.FECHA_PREVISTA_FABRICACION).IsModified = true;
                _context.Entry(programa).Property(p => p.FECHA_INICIO_REAL_FABRICACION).IsModified = true;
                _context.Entry(programa).Property(p => p.CANTFAB).IsModified = true;
                _context.Entry(programa).Property(p => p.FE_CIERRE).IsModified = true;
                _context.Entry(programa).Property(p => p.CG_ORDFORIG).IsModified = true;
                _context.Entry(programa).Property(p => p.CG_CELDA).IsModified = true;
                _context.Entry(programa).Property(p => p.PROCESO).IsModified = true;
                _context.Entry(programa).Property(p => p.CG_OPER).IsModified = true;
                _context.Entry(programa).Property(p => p.DES_OPER).IsModified = true;
                


                await _context.SaveChangesAsync();
                //await _context.Database.ExecuteSqlRawAsync(xSQL);
                return Ok();
            }
            catch (Exception ex)
            {
                 return BadRequest(ex);
            }

        }

        [HttpPut("PutFromModeloOF/{id}")]
        public async Task<IActionResult> PutFromFabricacion(int id, Fabricacion xItem)
        {
            if (id != xItem.CG_ORDF)
            {
                return BadRequest();
            }

            try
            {
                var programa = await _context.Programa.Where(p => p.CG_ORDF == id).FirstOrDefaultAsync();
                programa.CG_CELDA = xItem.CG_CELDA;
                programa.FE_PLAN = xItem.FE_PLAN;
                programa.ORDEN = xItem.ORDEN;
                programa.FE_CURSO = xItem.FE_CURSO;


                _context.Attach(programa);

                _context.Entry(programa).Property(p => p.CG_CELDA).IsModified = true;
                _context.Entry(programa).Property(p => p.FE_PLAN).IsModified = true;
                _context.Entry(programa).Property(p => p.CANTFAB).IsModified = true;
                _context.Entry(programa).Property(p => p.ORDEN).IsModified = true;
                _context.Entry(programa).Property(p => p.FE_CURSO).IsModified = true;



                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}
