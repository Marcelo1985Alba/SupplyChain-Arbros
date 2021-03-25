using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministracionArchivosController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        //Initialize the memory cache object   
        public IMemoryCache _cache;
        private readonly AppDbContext _context;
        private readonly ILogger<AdministracionArchivosController> logger;

        public AdministracionArchivosController(IWebHostEnvironment hostingEnvironment, IMemoryCache cache
            , AppDbContext appContext
            , ILogger<AdministracionArchivosController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
            this._context = appContext;
            this.logger = logger;
            logger.LogInformation("AdministracionArchivosController initialized");
        }

        [HttpGet("GetDocumentosByRuta/{parametro}/{codigo}")]
        public async Task<ActionResult<string[]>> GetDocumentosByRuta(string parametro, string codigo)
        {
            try
            {
                var ruta = await _context.Solution.Where(s => s.CAMPO == parametro).FirstOrDefaultAsync();
                string[] dirs = Directory.GetFiles(@$"{ruta.VALORC}", $"{codigo.Substring(0, 7)}*");

                return dirs;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
