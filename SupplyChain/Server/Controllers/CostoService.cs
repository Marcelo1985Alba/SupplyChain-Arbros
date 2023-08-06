using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;

namespace SupplyChain
{
    public class CostoService
    {
        private string CadenaConexionSQL;
        private readonly AppDbContext _context;
        private DataTable dbPlanificacion;

        public CostoService(AppDbContext context, string cadenaConexionSQL)
        {
            _context = context;
            CadenaConexionSQL = cadenaConexionSQL;
        }
        
        public async Task<decimal> CalcularCostoPorProd(string cg_prod, int formula, decimal cantidad)
        {
            try
            {
                Decimal? costo = 0;
                ConexionSQL xConexionSQL = new(CadenaConexionSQL);
                dbPlanificacion = xConexionSQL
                    .EjecutarSQL(String.Format("EXEC NET_PCP_Despiece_Producto '{0}', {1}, {2}", cg_prod, formula, cantidad));

                ConexionSQL xConexionSQL2 = new ConexionSQL(CadenaConexionSQL);
                string xSQL = "SELECT id, COTIZACION, FEC_ULT_ACT FROM ARBROS.dbo.ERP_COTIZACIONES";
                DataTable dbCotizaciones = xConexionSQL.EjecutarSQL(xSQL);
                List<Cotizaciones> cotizaciones = dbCotizaciones.AsEnumerable().Select(m => new Cotizaciones()
                {
                    Id = m.Field<int>("ID"),
                    COTIZACION = m.Field<double>("COTIZACION"),
                    FEC_ULT_ACT = m.Field<DateTime?>("FEC_ULT_ACT"),
                }).ToList<Cotizaciones>();

                List<DespiecePlanificacion> xLista = dbPlanificacion.AsEnumerable()
                    .Select(m => new DespiecePlanificacion()
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
                    COSTO = 0,
                }).ToList();
                
                foreach (DespiecePlanificacion item in xLista)
                {
                    var codigoInsumo = string.IsNullOrWhiteSpace(item.CG_SE) ? item.CG_MAT.Trim() : item.CG_SE.Trim();
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(item.CG_SE))
                        {
                            ConexionSQL xConexionSQL3 = new ConexionSQL(CadenaConexionSQL);
                            string xSQLCommandString2 = "SELECT * FROM FORM2 WHERE CG_PROD ='" + codigoInsumo + "' and REVISION  = (SELECT MAX(REVISION) FROM FORM2 " +
                                                   "WHERE CG_PROD = '" + codigoInsumo + "')";

                            dbPlanificacion = xConexionSQL3.EjecutarSQL(xSQLCommandString2);
                            item.formulasSemielaborado = dbPlanificacion.AsEnumerable().Select(m => new Shared.Formula()
                            {
                                Cg_Mat = m.Field<string>("CG_MAT"),
                                CANT_MAT = m.Field<decimal>("CANT_MAT"),
                                Cg_Prod = m.Field<string>("CG_PROD"),
                                Cg_Se = m.Field<string>("CG_SE"),
                                COSTO = 0,
                            }).ToList<Formula>();
                            
                            foreach (Formula mat in item.formulasSemielaborado)
                            {
                                if(!string.IsNullOrWhiteSpace(mat.Cg_Mat))
                                {
                                    //solo ordenes de compra
                                    Compra aux = await _context.Compras.Where(s => s.CG_MAT.Trim() == mat.Cg_Mat.Trim() && s.NUMERO > 0)
                                        .OrderByDescending(s => s.FE_EMIT).FirstOrDefaultAsync();
                                    if (aux != null)
                                    {
                                        if (aux.MONEDA.Trim().ToLower() == "dolares")
                                        {
                                            mat.COSTO = (aux.PRECIOTOT / aux.SOLICITADO);
                                            item.COSTO += mat.COSTO * mat.CANT_MAT;
                                        } else if (aux.MONEDA.Trim().ToLower() == "pesos") {
                                            double cot = cotizaciones.Where(s => s.FEC_ULT_ACT <= aux.FE_EMIT).MaxBy(s => s.FEC_ULT_ACT).COTIZACION;
                                            mat.COSTO = ((aux.PRECIOTOT / aux.SOLICITADO) / (decimal) cot);
                                            item.COSTO += mat.COSTO * mat.CANT_MAT;
                                        } else
                                            mat.COSTO = 0;
                                    } else
                                        mat.COSTO = 0;
                                }
                            }   
                        }
                        else if(!string.IsNullOrWhiteSpace(item.CG_MAT))
                        {
                            Compra aux = await _context.Compras.Where(s => s.CG_MAT.Trim() == item.CG_MAT.Trim() && s.NUMERO > 0)
                                .OrderByDescending(s => s.FE_EMIT).FirstOrDefaultAsync();
                            if (aux != null)
                            {
                                if (aux.MONEDA.Trim().ToLower() == "dolares")
                                {
                                    item.COSTO = (aux.PRECIOTOT / aux.SOLICITADO) * item.CANT_MAT; 
                                } else if (aux.MONEDA.Trim().ToLower() == "pesos") {
                                    double cot = cotizaciones.Where(s => s.FEC_ULT_ACT <= aux.FE_EMIT).MaxBy(s => s.FEC_ULT_ACT).COTIZACION;
                                    item.COSTO = ((aux.PRECIOTOT / aux.SOLICITADO) / (decimal) cot) * item.CANT_MAT;
                                } else
                                    item.COSTO = 0;
                            }
                        }
                        costo += item.COSTO;
                    }
                    catch (Exception ex)
                    {
                        item.formulasSemielaborado = new List<Formula>();
                    }
                }
                return costo.Value;
            }
            catch (Exception ex)
            {
                return new decimal(0);
            }
        }
    }
}