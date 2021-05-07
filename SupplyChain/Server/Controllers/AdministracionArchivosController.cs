using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        private string DocumentPath;
        private readonly AppDbContext _context;
        private readonly ILogger<AdministracionArchivosController> logger;

        public AdministracionArchivosController(IWebHostEnvironment hostingEnvironment, IMemoryCache cache
            , AppDbContext appContext
            , ILogger<AdministracionArchivosController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            this._context = appContext;
        }

        [HttpGet("ByParamRuta/{parametro}/{codigo}")]
        public async Task<IEnumerable<Archivo>> GetDocumentosByRuta(string parametro, string codigo)
        {
            try
            {
                var archivos = new List<Archivo>();
                var ruta = await _context.Solution.Where(s => s.CAMPO == parametro).FirstOrDefaultAsync();
                var endLength = ruta.CAMPO.Trim() == "RUTAENSAYO" ? 9 : 7;
                string[] dirs = Directory.GetFiles(@$"{ruta.VALORC}", $"{codigo.Substring(0, endLength)}*");
                int identificacion = 0;
                foreach (string item in dirs)
                {
                    identificacion++;
                    var archivo = new Archivo()
                    {
                        Id = identificacion,
                        Nombre = item.Substring(ruta.VALORC.Length),
                        Directorio = item,
                        
                        Contenido = parametro == "RUTACNC" ? System.IO.File.ReadAllLines(item) : null,
                        ContenidoByte = parametro == "RUTACNC" ? System.IO.File.ReadAllBytes(item): null
                        //ContenidoBase64 = "data:application/pdf;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(item))
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
                    sw.WriteLine($"{ordfab.Presion}");
                    sw.WriteLine($"{ordfab.DES_OPER}");
                }

                return Ok(ordfab);

            }
            catch (Exception Ex)
            {
                return BadRequest(Ex);
            }
        }

        [HttpGet("GetPdfNube/{fileName}/{ruta}")]
        public async Task<ActionResult<string>> GetPdfNube(string fileName, string ruta)
        {
            //Connection String of Storage Account
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=blazorpdfplano;AccountKey=rAVlVam+dx56pfQknsxOEC72f5HqXJkN9bLa3ExOHpNUOpj5kO7ohcKGx4IMWMfO/egA70yy/q15uCnVv471fA==;EndpointSuffix=core.windows.net";
            //Container Name
            string containerName = "pdf-planos";

            var cliente = new BlobContainerClient(connectionString, containerName);
            //Crea el contenedor si no existe
            await cliente.CreateIfNotExistsAsync();
            //cliente.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            var blobs = cliente.GetBlobsAsync();

            foreach (BlobItem blob in cliente.GetBlobs())
            {
                Console.WriteLine(blob.Name);

                var mete = blob.Metadata;

                
                var coty = blob.Properties.ContentType;
                var properties = blob.Properties;
                MemoryStream memoryStream = new MemoryStream(blob.Properties.ContentHash);
                DocumentPath = "data:application/pdf;base64," + Convert.ToBase64String(memoryStream.ToArray());
            }

            //CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            //CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            //CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            //CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            //MemoryStream memoryStream = new MemoryStream();
            //await cloudBlockBlob.DownloadToStreamAsync(memoryStream);
            //DocumentPath = "data:application/pdf;base64," + Convert.ToBase64String(memoryStream.ToArray());

            return DocumentPath;
        }
    }
}
