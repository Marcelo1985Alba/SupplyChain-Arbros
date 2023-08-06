using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using planificacionController = SupplyChain.Server.Controllers.PlanificacionController;

namespace SupplyChain.Server.Controllers.Ingenieria
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngenieriaController : ControllerBase
    {
        private string CadenaConexionSQL = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
        private readonly AppDbContext _context;
        private DataTable dbIngenieria;

        public IngenieriaController(AppDbContext appDbContext)
        {
            this._context = appDbContext;
        }

        // GET: api/Ingenieria/GetProductoFormulas
        [HttpGet("GetProductoFormulas")]
        public async Task<ActionResult<IEnumerable<vIngenieriaProductosFormulas>>> GetProductoFormulas()
        {
            //OC ABIERTAS
            try
            {
                ConexionSQL xConexionSQL = new(CadenaConexionSQL);
                dbIngenieria = xConexionSQL
                    .EjecutarSQL(String.Format("SELECT * FROM vIngenieria_Productos_Formulas"));

                List<vIngenieriaProductosFormulas> xLista = dbIngenieria.AsEnumerable()
                    .Select(m => new vIngenieriaProductosFormulas()
                    {
                        CG_PROD = m.Field<string>("CG_PROD"),
                        DES_PROD = m.Field<string>("DES_PROD"),
                        TIENE_FORM = m.Field<bool>("TIENE_FORM"),
                        FORM_ACTIVA = m.Field<bool>("FORM_ACTIVA"),
                        PRECIO = m.Field<decimal>("PRECIO"),
                    }).ToList();

                foreach (vIngenieriaProductosFormulas item in xLista)
                {
                    CostoService _costoService = new CostoService(_context, CadenaConexionSQL);
                    item.CMP = await _costoService.CalcularCostoPorProd(item.CG_PROD, 1, 1);
                }

                return await _context.vIngenieriaProductosFormulas.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
        // GET: api/Ingenieria/GetCostoByProd
        [HttpGet("GetCostoByProd/{cg_prod}/{formula}/{cant}")]
        public async Task<decimal> GetCostoByProd(string cg_prod, int formula, decimal cant)
        {
            try
            {
                CostoService _costoService = new CostoService(_context, CadenaConexionSQL);
                decimal toRet = await _costoService.CalcularCostoPorProd(cg_prod, formula, cant); 
                return toRet;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        [HttpGet("ByCodigoProducto/{cg_prod}")]
        public async Task<ActionResult<IEnumerable<vIngenieriaProductosFormulas>>> GetProductoFormulas(string cg_prod)
        {
            try
            {
                return await _context.vIngenieriaProductosFormulas
                    .Where(s => s.TIENE_FORM && s.FORM_ACTIVA && s.CG_PROD == cg_prod)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
        [HttpGet("GetValues")]
        public async Task<Costos> GetValues(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                Costos costos = new Costos();
                string formattedStartDate = startDate?.ToString("MM-dd-yyyy");
                string formattedEndDate = endDate?.ToString("MM-dd-yyyy");
                
                if (!startDate.HasValue || !endDate.HasValue || startDate >= endDate)
                    throw new ArgumentException("Fechas inválidas");
                
                ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);
                
                //Primero voy a obterner los egresos.
                string xSQL = $"SELECT SUM(SALDO_CO) AS TOTAL FROM ARBROS.dbo._ERP_MAYOR" +
                              $" where TIPO_INGRESO = 'EGRESOS' and FEC_ASI BETWEEN '{formattedStartDate}' AND '{formattedEndDate}' ";
                DataTable dbEgresos = xConexionSQL.EjecutarSQL(xSQL);
                costos.egresos = dbEgresos.Rows[0].Field<double>("TOTAL");
                
                //Ahora voy a obtener las unidades equivalentes.
                xSQL = $"SELECT SUM(PE.STOCK * PR.CG_DENSEG) as Total from pedidos as PE inner join prod as PR on PE.CG_ART = PR.CG_PROD" +
                       $" where PE.tipoo = 4 and PE.CG_ORDEN = 1 and FE_MOV BETWEEN '{formattedStartDate}' AND '{formattedEndDate}' ";
                DataTable dbUnidadesEquivalentes = xConexionSQL.EjecutarSQL(xSQL);
                costos.unidades_equivalentes = dbUnidadesEquivalentes.Rows[0].Field<decimal>("TOTAL");
                
                //Ahora voy a obtener el coeficiente.
                costos.coeficiente = costos.egresos / (double)costos.unidades_equivalentes;

                return costos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los valores: {ex.Message}");
            }
        }
    }
}
