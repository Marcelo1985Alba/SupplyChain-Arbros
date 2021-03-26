using System;
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

        public PlanificacionController(AppDbContext context)
        {
            _context = context;
        }
        // GET: api/Planificacion/{armado}/{emitidas}
        [HttpGet("{armado}/{emitidas}")]
        public List<Planificacion> Get(int armado, int emitidas)
        {

            try
            {
                ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);

                string xSQLCommandString = "SELECT A.SEM_ORIGEN, A.SEM_ABAST_PURO, A.SEM_ABAST, B.CG_ORDEN, A.CG_ORDF" +
                ", (CASE WHEN B.CG_ORDEN=1 THEN 'Producto' ELSE (CASE WHEN B.CG_ORDEN=2 " +
                "THEN 'Semi-Elaborado de Proceso' ELSE (CASE WHEN B.CG_ORDEN=3 THEN 'Semi-Elaborado' " +
                "ELSE (CASE WHEN B.CG_ORDEN=4 THEN 'Materia Prima' ELSE (CASE WHEN B.CG_ORDEN=10 THEN 'Insumo No Productivo / Articulo de Reventa' ELSE (CASE WHEN B.CG_ORDEN=11 THEN 'Herramental e Insumos Inventariables' ELSE (CASE WHEN B.CG_ORDEN=12 THEN 'Repuestos' ELSE (CASE WHEN B.CG_ORDEN=13 THEN 'Servicios' ELSE '' END) END) END) END) END) END) END) END) AS CLASE" +
                ", (CASE WHEN A.CG_R='' THEN 'Fabricación' ELSE (CASE WHEN A.CG_R='R' THEN 'Reproceso' ELSE (CASE WHEN A.CG_R='T' THEN 'Retrabajo' ELSE (CASE WHEN A.CG_R='S' THEN 'Seleccion' ELSE (CASE WHEN A.CG_R='A' THEN 'Armado' ELSE '' END) END) END) END) END) AS CG_R" +
                ", A.CG_ESTADOCARGA, A.CG_PROD, A.DES_PROD, A.CANT, A.CANTFAB, B.UNID, A.PEDIDO" +
                ", B.UNIDSEG, A.DIASFAB, RTRIM(LTRIM(A.CG_CELDA)) AS CG_CELDA, A.CG_FORM, A.FE_ENTREGA, A.FE_EMIT, A.FE_PLAN, A.FE_FIRME, A.FE_CURSO, A.FE_ANUL, A.FE_CIERRE" +
                " FROM Programa A, Prod B WHERE CG_REG>=2 AND" +
                " (A.Cg_Ordf = A.Cg_OrdfOrig OR A.Cg_OrdfOrig = 0) AND" +
                " A.Cg_prod = B.Cg_prod AND" +
                " (A.CG_ESTADOCARGA = 1";


                //var query = (from ordenes in _context.Programa
                //            join prod in _context.Prod on ordenes.CG_PROD equals prod.CG_PROD
                //            where ordenes.CG_ORDF == ordenes.CG_ORDFORIG || ordenes.CG_ORDFORIG == 0
                //            && ordenes.CG_REG == 2 && ordenes.CG_ESTADOCARGA == 1
                //            select new Planificacion()
                //            {
                //                CG_PROD = prod.CG_PROD,
                //                DES_PROD = prod.DES_PROD,
                //                CG_ORDEN = prod.CG_ORDEN,
                //                SEM_ORIGEN = ordenes.SEM_ORIGEN,
                //                SEM_ABAST_PURO = ordenes.SEM_ABAST_PURO,
                //                SEM_ABAST = ordenes.SEM_ABAST,
                //                CG_ORDF = ordenes.CG_ORDF,
                //                CLASE = prod.CG_ORDEN == 1 ? "Producto" :  prod.CG_ORDEN == 2 ? "Semi - Elaborado de Proceso" :
                //                        prod.CG_ORDEN == 3 ? "Semi-Elaborado" : prod.CG_ORDEN == 4 ? "Materia Prima" :
                //                        prod.CG_ORDEN == 10 ? "Insumo No Productivo / Articulo de Reventa" :
                //                        prod.CG_ORDEN == 11 ? "Herramental e Insumos Inventariables" :
                //                        prod.CG_ORDEN == 12 ? "Repuestos" :  "Servicios",
                //                CG_R = ordenes.CG_R == "" ? "Fabricación" :
                //                        ordenes.CG_R == "R" ? "Reporoceso" :
                //                        ordenes.CG_R == "T" ? "Retrabajo" :
                //                        ordenes.CG_R == "S" ? "Seleccion" :
                //                        ordenes.CG_R == "A" ? "Armado" : "",
                //                CG_ESTADOCARGA = ordenes.CG_ESTADOCARGA,
                //                CANT = ordenes.CANT,
                //                CANTFAB = ordenes.CANTFAB,
                //                UNID = prod.UNID,
                //                UNIDSEG = prod.UNIDSEG,
                //                PEDIDO = ordenes.PEDIDO,
                //                DIASFAB = ordenes.DIASFAB,
                //                CG_CELDA = ordenes.CG_CELDA,
                //                CG_FORM = ordenes.CG_FORM,
                //                FE_ENTREGA = ordenes.FE_ENTREGA,
                //                FE_EMIT = ordenes.FE_EMIT,
                //                FE_PLAN = ordenes.FE_PLAN,
                //                FE_FIRME = ordenes.FE_FIRME,
                //                FE_CURSO = ordenes.FE_CURSO,
                //                FE_ANUL = ordenes.FE_ANUL,
                //                FE_CIERRE = ordenes.FE_CIERRE
                //            }
                //   ).AsQueryable();

                
                if (emitidas == 1)
                {
                    xSQLCommandString = xSQLCommandString + " OR A.CG_ESTADOCARGA = 0";
                    //query = query.Where(p => p.CG_ESTADOCARGA == 0);
                }
                


                xSQLCommandString = xSQLCommandString + ") ";
                if (armado == 1)
                {
                    xSQLCommandString = xSQLCommandString + " AND A.CG_R != 'A'";
                    //query = query.Where(p => p.CLASE != "A");
                }
                xSQLCommandString = xSQLCommandString + "ORDER BY A.CG_ORDF DESC";

                //query = query.OrderByDescending(p => p.CG_ORDF);
                dbPlanificacion = xConexionSQL.EjecutarSQL(xSQLCommandString);

                List<Planificacion> xLista = dbPlanificacion.AsEnumerable().Select(m => new Planificacion()
                {
                    CG_PROD = m.Field<string>("CG_PROD"),
                    DES_PROD = m.Field<string>("DES_PROD"),
                    CG_ORDEN = m.Field<int>("CG_ORDEN"),
                    SEM_ORIGEN = m.Field<int>("SEM_ORIGEN"),
                    SEM_ABAST_PURO = m.Field<int>("SEM_ABAST_PURO"),
                    SEM_ABAST = m.Field<int>("SEM_ABAST"),
                    CG_ORDF = m.Field<int>("CG_ORDF"),
                    CLASE = m.Field<string>("CLASE"),
                    CG_R = m.Field<string>("CG_R"),
                    CG_ESTADOCARGA = m.Field<int>("CG_ESTADOCARGA"),
                    CANT = m.Field<decimal>("CANT"),
                    CANTFAB = m.Field<decimal>("CANTFAB"),
                    UNID = m.Field<string>("UNID"),
                    UNIDSEG = m.Field<string>("UNIDSEG"),
                    PEDIDO = m.Field<int>("PEDIDO"),
                    DIASFAB = m.Field<decimal>("DIASFAB"),
                    CG_CELDA = m.Field<string>("CG_CELDA"),
                    CG_FORM = m.Field<int>("CG_FORM"),
                    FE_ENTREGA = m.Field<DateTime?>("FE_ENTREGA"),
                    FE_EMIT = m.Field<DateTime?>("FE_EMIT"),
                    FE_PLAN = m.Field<DateTime?>("FE_PLAN"),
                    FE_FIRME = m.Field<DateTime?>("FE_FIRME"),
                    FE_CURSO = m.Field<DateTime?>("FE_CURSO"),
                    FE_ANUL = m.Field<DateTime?>("FE_ANUL"),
                    FE_CIERRE = m.Field<DateTime?>("FE_CIERRE"),
                }).ToList();

                //Para ver consulta SQL
                //var consulta = query.ToQueryString();
                //var xLista = await query.ToListAsync();

                return xLista;
            }
            catch (Exception ex)
            {
                return new List<Planificacion>();
            }
        }

        // GET: api/Planificacion/Despiece/{CG_PROD}/{FORMULA}/{CANTIDAD}
        [HttpGet("Despiece/{CG_PROD}/{FORMULA}/{CANTIDAD}")]
        public List<DespiecePlanificacion> Despiece(string cg_prod, int formula, decimal cantidad)
        {
            try
            {
                ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);
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
        public List<Planificacion> OrdenesCerradasYAnuladas(int Busqueda)
        {
            try
            {
                ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);

                string xSQLCommandString = $"SELECT TOP {Busqueda} B.CG_ORDEN, A.CG_ORDF" +
                                                         ", (CASE WHEN B.CG_ORDEN=1 THEN 'Producto' ELSE (CASE WHEN B.CG_ORDEN=2 THEN 'Semi-Elaborado de Proceso' ELSE (CASE WHEN B.CG_ORDEN=3 THEN 'Semi-Elaborado' ELSE (CASE WHEN B.CG_ORDEN=4 THEN 'Materia Prima' ELSE (CASE WHEN B.CG_ORDEN=10 THEN 'Insumo No Productivo / Articulo de Reventa' ELSE (CASE WHEN B.CG_ORDEN=11 THEN 'Herramental e Insumos Inventariables' ELSE (CASE WHEN B.CG_ORDEN=12 THEN 'Repuestos' ELSE (CASE WHEN B.CG_ORDEN=13 THEN 'Servicios' ELSE '' END) END) END) END) END) END) END) END) AS CLASE" +
                                                         ", (CASE WHEN A.CG_R='' THEN 'Productiva' ELSE (CASE WHEN A.CG_R='R' THEN 'Reproceso' ELSE (CASE WHEN A.CG_R='T' THEN 'Retrabajo' ELSE (CASE WHEN A.CG_R='S' THEN 'Seleccion' ELSE '' END) END) END) END) AS CG_R" +
                                                         ", A.CG_ESTADOCARGA, A.CG_PROD, A.DES_PROD, A.CANT, A.CANTFAB, B.UNID, A.PEDIDO" +
                                                         ", B.UNIDSEG, A.DIASFAB, A.CG_CELDA, A.CG_FORM, A.FE_ENTREGA, A.FE_EMIT, A.FE_PLAN, A.FE_FIRME, A.FE_CURSO" +
                                                         ", A.FE_ANUL, A.FE_CIERRE FROM Programa A, Prod B WHERE CG_REG>=2 AND" +
                                                         "  A.Cg_prod = B.Cg_prod AND CG_ESTADOCARGA IN (4, 5) ORDER BY A.CG_ORDF DESC";
                dbPlanificacion = xConexionSQL.EjecutarSQL(xSQLCommandString);

                List<Planificacion> xLista = dbPlanificacion.AsEnumerable().Select(m => new Planificacion()
                {
                    CG_PROD = m.Field<string>("CG_PROD"),
                    DES_PROD = m.Field<string>("DES_PROD"),
                    CG_ORDEN = m.Field<int>("CG_ORDEN"),
                    CG_ORDF = m.Field<int>("CG_ORDF"),
                    CLASE = m.Field<string>("CLASE"),
                    CG_R = m.Field<string>("CG_R"),
                    CG_ESTADOCARGA = m.Field<int>("CG_ESTADOCARGA"),
                    CANT = m.Field<decimal>("CANT"),
                    CANTFAB = m.Field<decimal>("CANTFAB"),
                    UNID = m.Field<string>("UNID"),
                    UNIDSEG = m.Field<string>("UNIDSEG"),
                    PEDIDO = m.Field<int>("PEDIDO"),
                    DIASFAB = m.Field<decimal>("DIASFAB"),
                    CG_CELDA = m.Field<string>("CG_CELDA"),
                    CG_FORM = m.Field<int>("CG_FORM"),
                    FE_ENTREGA = m.Field<DateTime?>("FE_ENTREGA"),
                    FE_EMIT = m.Field<DateTime?>("FE_EMIT"),
                    FE_PLAN = m.Field<DateTime?>("FE_PLAN"),
                    FE_FIRME = m.Field<DateTime?>("FE_FIRME"),
                    FE_CURSO = m.Field<DateTime?>("FE_CURSO"),
                    FE_ANUL = m.Field<DateTime?>("FE_ANUL"),
                    FE_CIERRE = m.Field<DateTime?>("FE_CIERRE"),
                }).ToList<Planificacion>();

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
            ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);
            if (pl.CG_ESTADOCARGA == 0)
            {
                xConexionSQL.EjecutarSQLNonQuery("UPDATE Programa SET CG_ESTADOCARGA = " + pl.CG_ESTADOCARGA + ",Fe_emit = GETDATE(), CG_ESTADO = " + ValorAnterior + " WHERE (Cg_ordf =" + pl.CG_ORDF +
                    " OR Cg_ordfAsoc = " + pl.CG_ORDF + ")");
            }
            else if (pl.CG_ESTADOCARGA == 1)
            {
                xConexionSQL.EjecutarSQLNonQuery("UPDATE Programa SET CG_ESTADOCARGA = " + pl.CG_ESTADOCARGA + ",Fe_plan = GETDATE(), CG_ESTADO = " + ValorAnterior + " WHERE (Cg_ordf =" + pl.CG_ORDF +
                                                                     " OR Cg_ordfAsoc = " + pl.CG_ORDF + ")");
            }
            else if (pl.CG_ESTADOCARGA == 5)
            {
                xConexionSQL.EjecutarSQLNonQuery("EXEC NET_PCP_Anular_OrdenFabricacion " + pl.CG_ORDF + ", 'User'");
            }
            return NoContent();
        }
    }
}
