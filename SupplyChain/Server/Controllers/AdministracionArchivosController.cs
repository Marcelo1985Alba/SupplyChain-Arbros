using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SupplyChain.Shared;
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
        public async Task<IEnumerable<Archivo>> GetDocumentosByRuta(string parametro, string codigo)
        {
            try
            {
                var archivos = new List<Archivo>();
                var ruta = await _context.Solution.Where(s => s.CAMPO == parametro).FirstOrDefaultAsync();
                string[] dirs = Directory.GetFiles(@$"{ruta.VALORC}", $"{codigo.Substring(0, 7)}*");
                int identificacion = 0;
                foreach (string item in dirs)
                {
                    identificacion++;
                    var archivo = new Archivo()
                    {
                        Id = identificacion,
                        Nombre = item.Substring(ruta.VALORC.Length),
                        Directorio = item,
                        Contenido = System.IO.File.ReadAllLines(item),
                        ContenidoByte = System.IO.File.ReadAllBytes(item),
                        ContenidoBase64 = "data:application/pdf;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(item))
                };

                    archivos.Add(archivo);
                }

                return archivos;

            }
            catch
            {
                return new List<Archivo>();
            }
        }

        [HttpPost("DownloadText")]
        public async Task<ActionResult<ModeloOrdenFabricacion>> DownloadText(ModeloOrdenFabricacion ordfab)
        {
            var ruta = await _context.Solution.Where(s => s.CAMPO == "RUTADATOS").FirstOrDefaultAsync();
            string fileName = ruta.VALORC + ordfab.PEDIDO + ".txt";
            try
            {
                // Check if file already exists. If yes, delete it.
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
                
                // Create a new file
                using (StreamWriter sw = System.IO.File.CreateText(fileName))
                {
                    sw.WriteLine($"{ordfab.Des_Cli.Trim()}");
                    sw.WriteLine($"{ordfab.PEDIDO}");
                    sw.WriteLine($"{ordfab.PEDIDO}");
                    sw.WriteLine($"{ordfab.DES_OPER}");
                }

                return Ok(ordfab);

            }
            catch (Exception Ex)
            {
                return BadRequest(Ex);
            }
        }
    }
}
