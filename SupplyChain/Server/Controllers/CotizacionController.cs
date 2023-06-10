using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CotizacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private string CadenaConexionSQL = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
        private readonly CotizacionRepository _cotizacionRepository;

        public CotizacionController(CotizacionRepository cotizacionRepository)
        {
            this._cotizacionRepository = cotizacionRepository;
        }
        
        // GET: api/Cotizacion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cotizaciones>>> GetCotizaciones()
        {
            try
            {
                ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);
                string xSQL = "SELECT id, COTIZACION, FEC_ULT_ACT FROM ARBROS.dbo.ERP_COTIZACIONES";
                DataTable dbCotizaciones = xConexionSQL.EjecutarSQL(xSQL);

                List<Cotizaciones> xLista = dbCotizaciones.AsEnumerable().Select(m => new Cotizaciones()
                {
                    Id = m.Field<int>("ID"),
                    COTIZACION = m.Field<double>("COTIZACION"),
                    FEC_ULT_ACT = m.Field<DateTime?>("FEC_ULT_ACT"),
                }).ToList<Cotizaciones>();

                return xLista;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
