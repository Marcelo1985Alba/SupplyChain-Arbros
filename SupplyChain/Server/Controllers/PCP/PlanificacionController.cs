﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChain;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanificacionController : ControllerBase
    {
        private string CadenaConexionSQL = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
        private readonly AppDbContext _context;
        private DataTable dbPlanificacion;
        private readonly StockCorregidoRepository _stockCorregidoRepository;

        public PlanificacionController(AppDbContext context, StockCorregidoRepository stockCorregidoRepository)
        {
            _context = context;
            this._stockCorregidoRepository = stockCorregidoRepository;
        }
        // GET: api/Planificacion/{armado}/{emitidas}
        [HttpGet("{armado}/{emitidas}")]
        public async Task<List<Planificacion>> GetAsync(int armado, int emitidas)
        {

            try
            {
                string xSQLCommandString = "SELECT A.SEM_ORIGEN, A.SEM_ABAST_PURO, A.SEM_ABAST, B.CG_ORDEN, A.CG_ORDF" +
                ", (CASE WHEN B.CG_ORDEN=1 THEN 'Producto' ELSE (CASE WHEN B.CG_ORDEN=2 " +
                "THEN 'Semi-Elaborado de Proceso' ELSE (CASE WHEN B.CG_ORDEN=3 THEN 'Semi-Elaborado' " +
                "ELSE (CASE WHEN B.CG_ORDEN=4 THEN 'Materia Prima' ELSE (CASE WHEN B.CG_ORDEN=10 THEN 'Insumo No Productivo / Articulo de Reventa' ELSE (CASE WHEN B.CG_ORDEN=11 THEN 'Herramental e Insumos Inventariables' ELSE (CASE WHEN B.CG_ORDEN=12 THEN 'Repuestos' ELSE (CASE WHEN B.CG_ORDEN=13 THEN 'Servicios' ELSE '' END) END) END) END) END) END) END) END) AS CLASE" +
                ", (CASE WHEN A.CG_R='' THEN 'Fabricación' ELSE (CASE WHEN A.CG_R='R' THEN 'Reproceso' ELSE (CASE WHEN A.CG_R='T' THEN 'Retrabajo' ELSE (CASE WHEN A.CG_R='S' THEN 'Seleccion' ELSE (CASE WHEN A.CG_R='A' THEN 'Armado' ELSE '' END) END) END) END) END) AS CG_R" +
                ", A.CG_ESTADOCARGA, A.CG_PROD, A.DES_PROD, A.CANT, A.CANTFAB, B.UNID, A.PEDIDO" +
                ", B.UNIDSEG, A.DIASFAB, RTRIM(LTRIM(A.CG_CELDA)) AS CG_CELDA, A.CG_FORM, A.FE_ENTREGA, A.FE_EMIT, A.FE_PLAN, " +
                "A.FE_FIRME, A.FE_CURSO, A.FE_ANUL, A.FE_CIERRE, B.UNIDEQUI * A.CANT as UNIDEQUI" +
                " FROM Programa A, Prod B WHERE CG_REG>=2 AND" +
                " (A.Cg_Ordf = A.Cg_OrdfOrig OR A.Cg_OrdfOrig = 0) AND" +
                " A.Cg_prod = B.Cg_prod AND" +
                " (A.CG_ESTADOCARGA = 1";

                
                if (emitidas == 1)
                    xSQLCommandString += " OR A.CG_ESTADOCARGA = 0) ";

                if (armado == 1)
                    xSQLCommandString += " AND A.CG_R != 'A'";

                xSQLCommandString += "ORDER BY A.CG_ORDF DESC";
                

                var xLista = await _context.Planificaciones.FromSqlRaw(xSQLCommandString).ToListAsync();

                return xLista;
            }
            catch (Exception ex)
            {
                return new List<Planificacion>();
            }
        }

        // GET: api/Planificacion/Despiece/{CG_PROD}/{FORMULA}/{CANTIDAD}
        [HttpGet("Despiece/{CG_PROD}/{FORMULA}/{CANTIDAD}")]
        public async Task<ActionResult<List<DespiecePlanificacion>>> Despiece(string cg_prod, int formula, decimal cantidad)
        {
            try
            {
                ConexionSQL xConexionSQL = new(CadenaConexionSQL);
                dbPlanificacion = xConexionSQL
                    .EjecutarSQL(String.Format("EXEC NET_PCP_Despiece_Producto '{0}', {1}, {2}", cg_prod, formula, cantidad));

                List<DespiecePlanificacion> xLista = dbPlanificacion.AsEnumerable().Select(m => new DespiecePlanificacion()
                {
                    CG_PROD = m.Field<string>("CG_PROD"),
                    CG_SE = m.Field<string>("CG_SE"),
                    CG_MAT = m.Field<string>("CG_MAT"),
                    DES_PROD = m.Field<string>("DES_PROD"),
                    UNID = m.Field<string>("UNID"),
                    CG_FORM = m.Field<decimal>("CG_FORM"),
                    STOCK = m.Field<decimal>("STOCK"),
                    CANT_MAT = m.Field<decimal>("CANT_MAT"),
                    SALDO = m.Field<decimal>("SALDO"),
                    CANT_PLANEADAS = m.Field<decimal>("CANT_PLANEADAS"),
                    SALDO_PLANEADAS = m.Field<decimal>("SALDO_PLANEADAS"),
                    CANT_TOTAL = m.Field<decimal>("CANT_TOTAL"),
                    SALDO_TOTAL = m.Field<decimal>("SALDO_TOTAL"),
                }).ToList();


                
                foreach (DespiecePlanificacion item in xLista)
                {
                    var codigoInsumo = string.IsNullOrWhiteSpace(item.CG_SE) ? item.CG_MAT.Trim() : item.CG_SE.Trim();
                    item.ResumenStocks = await _context.ResumenStock
                        .Where(r => r.CG_ART == codigoInsumo && r.STOCK > 0)
                        .ToListAsync();
                    item.StockCorregido = await _stockCorregidoRepository.Obtener(s=> s.CG_PROD == codigoInsumo).FirstOrDefaultAsync();
                }

                return xLista;
            }
            catch (Exception ex)
            {
                return new List<DespiecePlanificacion>();
            }
        }
        // GET: api/Planificacion/Formula/{CG_PROD}
        [HttpGet("Formula/{CG_PROD}")]
        public List<FormulaPlanificacion> Formula(string cg_prod)
        {
            try
            {
                ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);
                string xSQLCommandString = "SELECT CG_FORM, IIF(ACTIVO != 'N',STR(CG_FORM) + ' - ACTIVA',STR(CG_FORM)) AS DES_FORM, ACTIVO, MAX(REVISION) AS REVISION " +
                                       "FROM FORM2 WHERE CG_PROD = '" + cg_prod.Trim() + "' GROUP BY CG_FORM, ACTIVO";
                dbPlanificacion = xConexionSQL.EjecutarSQL(xSQLCommandString);
                List<FormulaPlanificacion> xLista = dbPlanificacion.AsEnumerable().Select(m => new FormulaPlanificacion()
                {
                    CG_FORM = m.Field<int>("CG_FORM"),
                    DES_FORM = m.Field<string>("DES_FORM"),
                    ACTIVO = m.Field<string>("ACTIVO"),
                    REVISION = m.Field<int>("REVISION"),
                }).ToList<FormulaPlanificacion>();

                return xLista;
            }
            catch (Exception ex)
            {
                return new List<FormulaPlanificacion>();
            }
        }
        // PUT: api/Planificacion/PutFormula/{cg_form}
        [HttpPut("PutFormula/{cg_form}")]
        public async Task<ActionResult<List<Planificacion>>> PutFormula(int cg_form, Planificacion pl)
        {
            ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);
            string xSQLcommandString = "UPDATE Programa SET Cg_form = " + cg_form.ToString() + " WHERE Cg_ordf = " + pl.CG_ORDF;
            xConexionSQL.EjecutarSQLNonQuery(xSQLcommandString);
            return NoContent();
        }
        // GET: api/Planificacion/OrdenesCerradasYAnuladas/{Busqueda}
        [HttpGet("OrdenesCerradasYAnuladas/{Busqueda}")]
        public async Task<List<Planificacion>> OrdenesCerradasYAnuladas(int Busqueda)
        {
            try
            {
                string xSQLCommandString = "SELECT TOP " + Busqueda+ " A.SEM_ORIGEN, A.SEM_ABAST_PURO, A.SEM_ABAST, B.CG_ORDEN, A.CG_ORDF" +
                ", (CASE WHEN B.CG_ORDEN=1 THEN 'Producto' ELSE (CASE WHEN B.CG_ORDEN=2 " +
                "THEN 'Semi-Elaborado de Proceso' ELSE (CASE WHEN B.CG_ORDEN=3 THEN 'Semi-Elaborado' " +
                "ELSE (CASE WHEN B.CG_ORDEN=4 THEN 'Materia Prima' ELSE (CASE WHEN B.CG_ORDEN=10 THEN 'Insumo No Productivo / Articulo de Reventa' ELSE (CASE WHEN B.CG_ORDEN=11 THEN 'Herramental e Insumos Inventariables' ELSE (CASE WHEN B.CG_ORDEN=12 THEN 'Repuestos' ELSE (CASE WHEN B.CG_ORDEN=13 THEN 'Servicios' ELSE '' END) END) END) END) END) END) END) END) AS CLASE" +
                ", (CASE WHEN A.CG_R='' THEN 'Fabricación' ELSE (CASE WHEN A.CG_R='R' THEN 'Reproceso' ELSE (CASE WHEN A.CG_R='T' THEN 'Retrabajo' ELSE (CASE WHEN A.CG_R='S' THEN 'Seleccion' ELSE (CASE WHEN A.CG_R='A' THEN 'Armado' ELSE '' END) END) END) END) END) AS CG_R" +
                ", A.CG_ESTADOCARGA, A.CG_PROD, A.DES_PROD, A.CANT, A.CANTFAB, B.UNID, A.PEDIDO" +
                ", B.UNIDSEG, A.DIASFAB, RTRIM(LTRIM(A.CG_CELDA)) AS CG_CELDA, A.CG_FORM, A.FE_ENTREGA, A.FE_EMIT, A.FE_PLAN, " +
                "A.FE_FIRME, A.FE_CURSO, A.FE_ANUL, A.FE_CIERRE, B.UNIDEQUI * A.CANT as UNIDEQUI" +
                " FROM Programa A, Prod B WHERE CG_REG>=2 AND" +
                "  A.Cg_prod = B.Cg_prod AND CG_ESTADOCARGA IN (4, 5) ORDER BY A.CG_ORDF DESC";

                var xLista = await _context.Planificaciones.FromSqlRaw(xSQLCommandString).ToListAsync();
                return xLista;
            }
            catch (Exception ex)
            {
                return new List<Planificacion>();
            }
        }
        // PUT: api/Planificacion/PutPlanif/{ValorAnterior}
        [HttpPut("PutPlanif/{ValorAnterior}")]
        public async Task<ActionResult<List<Planificacion>>> PutPlanif(int ValorAnterior, Planificacion pl)
        {
            //ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);

            var query = "";
            
            if (pl.CG_ESTADOCARGA == 0)
            {
                query = "UPDATE Programa SET CG_ESTADOCARGA = " + pl.CG_ESTADOCARGA +
                    ",Fe_emit = GETDATE(), CG_ESTADO = " + ValorAnterior + " WHERE (Cg_ordf =" + pl.CG_ORDF +
                    " OR Cg_ordfAsoc = " + pl.CG_ORDF + ")";
            }
            else if (pl.CG_ESTADOCARGA == 1)
            {
                query = "UPDATE Programa SET CG_ESTADOCARGA = " + pl.CG_ESTADOCARGA + 
                    ",Fe_plan = GETDATE(), CG_ESTADO = " + ValorAnterior + " WHERE (Cg_ordf =" + pl.CG_ORDF +
                 " OR Cg_ordfAsoc = " + pl.CG_ORDF + ")";
            }
            else if (pl.CG_ESTADOCARGA == 2)
            {
                query = "UPDATE Programa SET CG_ESTADOCARGA = " + pl.CG_ESTADOCARGA +
                    ",Fe_Firme = GETDATE(), CG_ESTADO = " + ValorAnterior + " WHERE (Cg_ordf =" + pl.CG_ORDF +
                 " OR Cg_ordfAsoc = " + pl.CG_ORDF + ")";
            }
            else if (pl.CG_ESTADOCARGA == 5)
            {
                query = "EXEC NET_PCP_Anular_OrdenFabricacion " + pl.CG_ORDF + ", 'User'";
            }

            await _context.Database.ExecuteSqlRawAsync(query);
            return Ok();
        }
    }
}
