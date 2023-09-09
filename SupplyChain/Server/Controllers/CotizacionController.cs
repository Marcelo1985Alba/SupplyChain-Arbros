using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CotizacionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CotizacionRepository _cotizacionRepository;

    private readonly string CadenaConexionSQL = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");

    public CotizacionController(CotizacionRepository cotizacionRepository)
    {
        _cotizacionRepository = cotizacionRepository;
    }

    // GET: api/Cotizacion
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cotizaciones>>> GetCotizaciones()
    {
        try
        {
            var xConexionSQL = new ConexionSQL(CadenaConexionSQL);
            var xSQL = "SELECT id, COTIZACION, FEC_ULT_ACT FROM ARBROS.dbo.ERP_COTIZACIONES";
            var dbCotizaciones = xConexionSQL.EjecutarSQL(xSQL);

            var xLista = dbCotizaciones.AsEnumerable().Select(m => new Cotizaciones
            {
                Id = m.Field<int>("ID"),
                COTIZACION = m.Field<double>("COTIZACION"),
                FEC_ULT_ACT = m.Field<DateTime?>("FEC_ULT_ACT")
            }).ToList();

            return xLista;
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}