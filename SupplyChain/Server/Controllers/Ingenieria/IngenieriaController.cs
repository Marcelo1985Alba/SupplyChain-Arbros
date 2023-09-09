using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChain.Shared;
using planificacionController = SupplyChain.Server.Controllers.PlanificacionController;

namespace SupplyChain.Server.Controllers.Ingenieria;

[Route("api/[controller]")]
[ApiController]
public class IngenieriaController : ControllerBase
{
    private readonly AppDbContext _context;

    private readonly string CadenaConexionSQL = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build()
        .GetConnectionString("DefaultConnection");

    private DataTable dbIngenieria;

    public IngenieriaController(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    // GET: api/Ingenieria/GetProductoFormulas
    [HttpGet("GetProductoFormulas")]
    public async Task<ActionResult<IEnumerable<vIngenieriaProductosFormulas>>> GetProductoFormulas()
    {
        //OC ABIERTAS
        try
        {
            return await _context.vIngenieriaProductosFormulas.ToListAsync();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // GET: api/Ingenieria/GetProductoFormulasWithCost
    [HttpGet("GetProductoFormulasWithCost")]
    public async Task<ActionResult<IEnumerable<vIngenieriaProductosFormulas>>> GetProductoFormulasWithCost(
        DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var toRet = await _context.vIngenieriaProductosFormulas.ToListAsync();
            var _costoService = new CostoService(_context, CadenaConexionSQL);
            foreach (var item in toRet) item.COSTO = await _costoService.CalcularCostoPorProd(item.CG_PROD, 1, 1);

            return toRet;
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
            var _costoService = new CostoService(_context, CadenaConexionSQL);
            var toRet = await _costoService.CalcularCostoPorProd(cg_prod, formula, cant);
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
            var costos = new Costos();
            var formattedStartDate = startDate?.ToString("MM-dd-yyyy");
            var formattedEndDate = endDate?.ToString("MM-dd-yyyy");

            if (!startDate.HasValue || !endDate.HasValue || startDate >= endDate)
                throw new ArgumentException("Fechas inválidas");

            var xConexionSQL = new ConexionSQL(CadenaConexionSQL);

            //Primero voy a obterner los egresos.
            var xSQL = $"SELECT SUM(SALDO_CO) AS TOTAL FROM ARBROS.dbo._ERP_MAYOR" +
                       $" where TIPO_INGRESO = 'EGRESOS' and FEC_ASI BETWEEN '{formattedStartDate}' AND '{formattedEndDate}' ";
            var dbEgresos = xConexionSQL.EjecutarSQL(xSQL);
            costos.egresos = dbEgresos.Rows[0].Field<double>("TOTAL");

            //Ahora voy a obtener las unidades equivalentes.
            xSQL =
                $"SELECT SUM(PE.STOCK * PR.CG_DENSEG) as Total from pedidos as PE inner join prod as PR on PE.CG_ART = PR.CG_PROD" +
                $" where PE.tipoo = 4 and PE.CG_ORDEN = 1 and FE_MOV BETWEEN '{formattedStartDate}' AND '{formattedEndDate}' ";
            var dbUnidadesEquivalentes = xConexionSQL.EjecutarSQL(xSQL);
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