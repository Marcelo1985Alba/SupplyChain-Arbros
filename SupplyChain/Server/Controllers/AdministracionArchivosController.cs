using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.PdfViewer;
using Syncfusion.Pdf;
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
        private readonly ILogger<AdministracionArchivosController> logger;
        private readonly SolutionRepository _solutionRepository;
        private readonly AppDbContext _context;

        public AdministracionArchivosController(IWebHostEnvironment hostingEnvironment, IMemoryCache cache
            , SolutionRepository solutionRepository, AppDbContext context
            , ILogger<AdministracionArchivosController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            this._solutionRepository = solutionRepository;
            _context = context;
            this._cache = cache;
        }

        [HttpGet("ByParamRuta/{parametro}/{codigo}")]
        public async Task<ActionResult<List<Archivo>>> GetDocumentosByRuta(string parametro, string codigo)
        {
            try
            {
                var ruta = await _solutionRepository.Obtener(s => s.CAMPO == parametro).FirstOrDefaultAsync();
                if (ruta.VALORC.Contains("blob"))
                {
                    string pedido = codigo.Split(',')[0];

                    const string FirstCharacter = "_";
                    int Pos1 = pedido.IndexOf(FirstCharacter) + FirstCharacter.Length;
                    pedido = pedido.Substring(Pos1);

                    int Pos2 = pedido.IndexOf('_') + FirstCharacter.Length;
                    pedido = pedido.Substring(0, Pos2 - 1);
                    return await GetEnsayosAzure(Convert.ToInt32(pedido));
                }
                else
                {
                    return await GetFileLocalServer(parametro, codigo);
                }
                

            }
            catch (Exception ex)
            {
                return new List<Archivo>();
            }
        }

        private async Task<List<Archivo>> GetFileLocalServer(string parametro, string codigo)
        {
            var archivos = new List<Archivo>();
            var ruta = await _solutionRepository.Obtener(s => s.CAMPO == parametro).FirstOrDefaultAsync();
            var endLength = ruta.CAMPO.Trim() == "RUTAENSAYO" ? 9 :
                    ruta.CAMPO.Trim() == "RUTACERTIFICADOS" || ruta.CAMPO.Trim() == "RUTATRAZABILIDAD"
                    || ruta.CAMPO.Trim() == "RUTAREMITO" || ruta.CAMPO.Trim() == "RUTAFACTURA" ? codigo.Length : 7;
            codigo = codigo.Split(',')[0];
            var file = codigo.Substring(0, endLength);
            if (ruta.CAMPO.Trim() == "RUTAENSAYO")
            {
                file += "_*.pdf";
            }
            else if (ruta.CAMPO.Trim() == "RUTATRAZABILIDAD")
            {
                file = codigo;
            }
            else if (ruta.CAMPO.Trim() == "RUTACERTIFICADOS")
            {
                file += "*.pdf";
            }
            else if (ruta.CAMPO.Trim() == "RUTAREMITO" || ruta.CAMPO.Trim() == "RUTAFACTURA")
            {
                file += "*.*";
            }

            string[] dirs = Directory.GetFiles($"{ruta.VALORC}",$"{file}",
                new EnumerationOptions() { MatchCasing = MatchCasing.CaseInsensitive, MatchType = MatchType.Simple, IgnoreInaccessible = true});
            int identificacion = 0;
            foreach (string item in dirs)
            {
                identificacion++;
                var archivo = new Archivo()
                {
                    Id = identificacion,
                    Nombre = Path.GetFileName(item),
                    Directorio = item,
                    Contenido = parametro == "RUTACNC" ? System.IO.File.ReadAllLines(item) : null,
                    ContenidoByte = parametro == "RUTACNC" || ruta.CAMPO.Trim() == "RUTACERTIFICADOS" || ruta.CAMPO.Trim() == "RUTATRAZABILIDAD"
                                    || ruta.CAMPO.Trim() == "RUTAFACTURA" || ruta.CAMPO.Trim() == "RUTAREMITO" ? System.IO.File.ReadAllBytes(item) : null
                    //ContenidoBase64 = "data:application/pdf;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(item))
                };

                archivos.Add(archivo);
            }


            return archivos;
        }

        [HttpPost("DownloadText")]
        public async Task<ActionResult<ModeloOrdenFabricacion>> DownloadText(ModeloOrdenFabricacion ordfab)
        {
            var ruta = await _solutionRepository.Obtener(s => s.CAMPO == "RUTADATOS").FirstOrDefaultAsync();
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

        [HttpGet("ExisteEspecificacion/{file}")]
        public async Task<bool> ExisteEspecificacion(string file)
        {
            var param = await _solutionRepository.Obtener(s => s.CAMPO == "RUTAESP").FirstOrDefaultAsync();
            var path = param.VALORC.Trim();

            return System.IO.File.Exists(path + "/" + file);
        }

        [HttpGet("ExistePlano/{file}")]
        public async Task<bool> ExistePlano(string file)
        {
            try
            {
                var param = await _solutionRepository.Obtener(s => s.CAMPO == "RUTAOF").FirstOrDefaultAsync();
                var path = param.VALORC.Trim();

                return System.IO.File.Exists(path + "/" + file);
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
        [HttpGet("ExisteCertificado/{file}")]
        public async Task<bool> ExisteCertificado(string file)
        {
            var param = await _solutionRepository.Obtener(s => s.CAMPO == "RUTATRAZABILIDAD").FirstOrDefaultAsync();
            var path = param.VALORC.Trim();

            if (path.Contains("blob", StringComparison.OrdinalIgnoreCase))
            {
                //BUSCAR EN AZURE
                var archivos = await GetCertificadoAzure(file);
                return archivos.Count > 0;
            }

            return System.IO.File.Exists(path + "/" + file);
        }


        [HttpGet("Certificado/{file}")]
        public async Task<List<Archivo>> Certificado(string file)
        {
            var param = await _solutionRepository.Obtener(s => s.CAMPO == "RUTATRAZABILIDAD").FirstOrDefaultAsync();
            var path = param.VALORC.Trim();

            if (path.Contains("blob", StringComparison.OrdinalIgnoreCase))
            {
                //BUSCAR EN AZURE
                var archivos = await GetCertificadoAzure(file);
                return archivos;
            }
            //TODO: OBTENER ARCHIVOS DE LOCALSERVER
            
            return await GetFileLocalServer("RUTATRAZABILIDAD", file);
        }


        [HttpGet("GetCertificado/{file}")]
        public async Task<List<Archivo>> GetCertificadoAzure(string file)
        {
            BlobContainerClient cliente = await GetContainer("certmp");
            var certificados = cliente.GetBlobs(prefix: file).ToList();
            List<Archivo> archivos = new();
            foreach (BlobItem blob in certificados)
            {
                var blobsa = cliente.GetBlobClient(blob.Name);
                MemoryStream memoryStream = new();
                await blobsa.DownloadToAsync(memoryStream);

                var archivo = new Archivo()
                {
                    Id = archivos.Count + 1,
                    Nombre = blob.Name,
                    ContenidoByte = memoryStream.ToArray(),
                    DocumentPath = "data:application/pdf;base64," + Convert.ToBase64String(memoryStream.ToArray()),
                    IsAzure = true
                };
                archivos.Add(archivo);
            }
            return archivos;
        }

        [HttpGet("GetPlano/{file}/Load")]
        public async Task<IActionResult> GetPlano(string file)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                var ruta = await _solutionRepository.Obtener(s => s.CAMPO == "RUTAOF").FirstOrDefaultAsync();
                byte[] bytes = System.IO.File.ReadAllBytes(ruta.VALORC + "/" + file);
                stream = new MemoryStream(bytes);
                string mimeType = "application/pdf";
                return new FileStreamResult(stream, mimeType)
                {
                    FileDownloadName = file
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetTxt/{filename}")]
        public async Task<IActionResult> GetTxt(string filename)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                var ruta = @"C:\Temp";
                byte[] bytes = System.IO.File.ReadAllBytes(ruta + "/" + filename);
                stream = new MemoryStream(bytes);
                string mimeType = "application/txt";
                return new FileStreamResult(stream, mimeType)
                {
                    FileDownloadName = filename
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetEnsayos/{pedido}")]
        public async Task<List<Archivo>> GetEnsayosAzure(int pedido)
        {
            return await GetBlobEnsayosAzure(pedido);
        }

        private async Task<List<Archivo>> GetBlobEnsayosAzure(int pedido)
        {
            BlobContainerClient cliente = await GetContainer("ensayos");
            //cliente.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            var prefijo = $"ENS_{pedido}";
            var ensayos = cliente.GetBlobs(prefix: prefijo).ToList();
            List<Archivo> archivos = new();
            foreach (BlobItem blob in ensayos)
            {
                //var mete = blob.Metadata;
                //var coty = blob.Properties.ContentType;
                //var properties = blob.Properties;
                var blobsa = cliente.GetBlobClient(blob.Name);
                MemoryStream memoryStream = new();
                await blobsa.DownloadToAsync(memoryStream);

                var archivo = new Archivo()
                {
                    Id = archivos.Count + 1,
                    Nombre = blob.Name,
                    ContenidoByte = memoryStream.ToArray(),
                    DocumentPath = "data:application/pdf;base64," + Convert.ToBase64String(memoryStream.ToArray()),
                    IsAzure = true
                };
                archivos.Add(archivo);
            }

            return archivos.Where(a => a.Nombre.Contains(prefijo)).ToList();
        }

        private static async Task<BlobContainerClient> GetContainer(string containerName)
        {
            //Connection String of Storage Account
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=arbrosstorage;AccountKey=d757aVPtZ0UFLcPfYn21vjEm4+FycBmm8q6OkvADT4lxWgBASGp/NeTR7eKYRBki4LwXF4RgfTRspa20VMxm/A==;EndpointSuffix=core.windows.net";


            var cliente = new BlobContainerClient(connectionString, containerName);
            //Crea el contenedor si no existe
            await cliente.CreateIfNotExistsAsync();
            return cliente;
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
                MemoryStream memoryStream = new(blob.Properties.ContentHash);
                DocumentPath = "data:application/pdf;base64," + Convert.ToBase64String(memoryStream.ToArray());
            }


            return DocumentPath;
        }

        [HttpPost("GuardarExcel")]
        public async Task<ActionResult<Archivo>> GuardarExcel(Archivo archivo)
        {
            if (archivo == null)
            {
                return BadRequest("El archivo es nulo");
            }

            if (archivo.ContenidoByte == null)
            {
                return BadRequest("Debe asignar stream");
            }

            string path = await _solutionRepository.Obtener(s=> s.CAMPO == "RUTADESPIECE").Select(s=> s.VALORC).FirstOrDefaultAsync();



            using (var memoryStream = new MemoryStream(archivo.ContenidoByte))
            {

                string tempFilePath = Path.Combine(path + archivo.Nombre);
                using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.WriteTo(fs);
                }

            }

            return Ok(archivo);
        }

        [HttpPost("Load")]
        //
        [Route("[controller]/Load")]
        //Post action for Loading the PDF documents   
        public async Task<IActionResult> Load([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF viewer object with memory cache object
            PdfRenderer pdfviewer = new(_cache);
            MemoryStream stream = new();
            object jsonResult = new object();
            if (jsonObject != null && jsonObject.ContainsKey("document"))
            {
                if (bool.Parse(jsonObject["isFileName"]))
                {

                    Archivo archivo = new();
                    var contenido = jsonObject["document"].Split(',');
                    string primera = contenido[0];
                    if (char.IsDigit(primera[0]))
                    {
                        var pedido = contenido[0];
                        var fileName = contenido[1];
                        var files = await GetBlobEnsayosAzure(Convert.ToInt32(pedido));
                        archivo = files.Find(f => f.Nombre == fileName);
                    }
                    else
                    {
                        var file = contenido[0];
                        archivo = (await GetCertificadoAzure(file)).FirstOrDefault();
                    }

                    
                    if (!string.IsNullOrEmpty(archivo.Nombre))
                    {
                        //byte[] bytes = System.IO.File.ReadAllBytes(documentPath);
                        stream = new MemoryStream(archivo.ContenidoByte);
                    }
                    else
                    {
                        return this.Content(jsonObject["document"] + " is not found");
                    }
                }
                else
                {
                    byte[] bytes = Convert.FromBase64String(jsonObject["document"]);
                    stream = new MemoryStream(bytes);
                }
            }

            jsonResult = pdfviewer.Load(stream, jsonObject);

            var result = Content(JsonConvert.SerializeObject(jsonResult));
            return result;
        }

        [AcceptVerbs("Post")]
        [HttpPost("RenderPdfPages")]

        [Route("[controller]/RenderPdfPages")]
        //Post action for processing the PDF documents  
        public IActionResult RenderPdfPages([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new(_cache);
            object jsonResult = pdfviewer.GetPage(jsonObject);
            var result = JsonConvert.SerializeObject(jsonResult);

            //var bytes = Encoding.UTF8.GetBytes(jsonResult.ToString());
            //return File(jsonResult, "application/octet-stream");
            return Content(result);
        }


        [AcceptVerbs("Post")]
        [HttpPost("Bookmarks")]

        [Route("[controller]/Bookmarks")]
        //Post action for processing the bookmarks from the PDF documents
        public IActionResult Bookmarks([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            var jsonResult = pdfviewer.GetBookmarks(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }



        [AcceptVerbs("Post")]
        [HttpPost("RenderThumbnailImages")]

        [Route("[controller]/RenderThumbnailImages")]
        //Post action for rendering the ThumbnailImages
        public IActionResult RenderThumbnailImages([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object result = pdfviewer.GetThumbnailImages(jsonObject);
            return Content(JsonConvert.SerializeObject(result));
        }
        [AcceptVerbs("Post")]
        [HttpPost("RenderAnnotationComments")]

        [Route("[controller]/RenderAnnotationComments")]
        //Post action for rendering the annotations
        public IActionResult RenderAnnotationComments([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new(_cache);
            object jsonResult = pdfviewer.GetAnnotationComments(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }
        [AcceptVerbs("Post")]
        [HttpPost("ExportAnnotations")]

        [Route("[controller]/ExportAnnotations")]
        //Post action to export annotations
        public IActionResult ExportAnnotations([FromBody] Dictionary<string, string> jsonObject)
        {
            //PdfRenderer pdfviewer = new PdfRenderer(_cache);
            //string jsonResult = pdfviewer.GetAnnotations(jsonObject);
            return NoContent();
        }
        [AcceptVerbs("Post")]
        [HttpPost("ImportAnnotations")]

        [Route("[controller]/ImportAnnotations")]
        //Post action to import annotations
        public IActionResult ImportAnnotations([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new(_cache);
            string jsonResult = string.Empty;
            if (jsonObject != null && jsonObject.ContainsKey("fileName"))
            {
                string documentPath = "";//GetDocumentPath(jsonObject["fileName"]);
                if (!string.IsNullOrEmpty(documentPath))
                {
                    jsonResult = System.IO.File.ReadAllText(documentPath);
                }
                else
                {
                    return this.Content(jsonObject["document"] + " is not found");
                }
            }
            return Content(jsonResult);
        }

        [AcceptVerbs("Post")]
        [HttpPost("ExportFormFields")]

        [Route("[controller]/ExportFormFields")]
        public IActionResult ExportFormFields([FromBody] Dictionary<string, string> jsonObject)

        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = pdfviewer.ExportFormFields(jsonObject);
            return Content(jsonResult);
        }

        [AcceptVerbs("Post")]
        [HttpPost("ImportFormFields")]

        [Route("[controller]/ImportFormFields")]
        public IActionResult ImportFormFields([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.ImportFormFields(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("Unload")]

        [Route("[controller]/Unload")]
        //Post action for unloading and disposing the PDF document resources  
        public IActionResult Unload([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            pdfviewer.ClearCache(jsonObject);
            return this.Content("Document cache is cleared");
        }


        [HttpPost("Download")]

        [Route("[controller]/Download")]
        //Post action for downloading the PDF documents
        public IActionResult Download([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new(_cache);
            string documentBase = pdfviewer.GetDocumentAsBase64(jsonObject);
            return Content(documentBase);
        }

        [HttpPost("PrintImages")]

        [Route("[controller]/PrintImages")]
        //Post action for printing the PDF documents
        public IActionResult PrintImages([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object pageImage = pdfviewer.GetPrintImage(jsonObject);
            return Content(JsonConvert.SerializeObject(pageImage));
        }

        [HttpPost("RenderPdfTexts")]
        [Route("[controller]/RenderPdfTexts")]
        public IActionResult RenderPdfTexts([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object result = pdfviewer.GetDocumentText(jsonObject);
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpGet("Descargar/{archivo}")]
        public async Task<FileResult> Descargar(Archivo archivo)
        {
            //List<Archivo> archivos = await GetFileLocalServer("RUTACERTIFICADOS", archivo.ToString());
            //return archivos;
            return File(archivo.ContenidoByte, System.Net.Mime.MediaTypeNames.Application.Pdf, archivo.Nombre);
        }

        [HttpGet("MergePdf/{nroPedido}")]
        public async Task<FileResult> MergePdf(int nroPedido)
        {
            var listaTrazabilidad = await _context.VTrazabilidads.Where(v => v.PEDIDO == nroPedido).ToListAsync();
            var listArchivosDescargar = new List<Archivo>();
            List<vTrazabilidad> lineasCertif = new();
            var lineasCertificados = await GetFileLocalServer("RUTACERTIFICADOS", nroPedido.ToString());
            listArchivosDescargar.Add(lineasCertificados[0]);
            if (listaTrazabilidad.Count > 0)
            {
                var lineasRoscada = new List<int>(new int[] { 8, 18, 52, 23 });
                var lineasBridada = new List<int>(new int[] { 8, 18, 23, 19 });
                var producto = listaTrazabilidad.FirstOrDefault(t => t.TIPOO == 1).CG_ART;
                if (producto.StartsWith("00"))//reparacion
                {

                }
                else if (producto.StartsWith("1")) //roscada
                {
                    lineasCertif = listaTrazabilidad.Where(t => lineasRoscada.Contains((int)t.CG_LINEA)).ToList();
                }
                else if (producto.StartsWith("2")) //bridada
                {
                    lineasCertif = listaTrazabilidad.Where(t => lineasBridada.Contains((int)t.CG_LINEA)).ToList();
                }
                if (lineasCertif.Count > 0)
                {
                    foreach (var item in lineasCertif)
                    {
                        var responseMp = await GetFileLocalServer("RUTATRAZABILIDAD", $"{item.DESPACHO}.pdf");
                        listArchivosDescargar.Add(responseMp[0]);
                    }
                }
            }
            Stream[] streams = new Stream[listArchivosDescargar.Count];
            PdfDocument finalDoc = new PdfDocument();
            for (int i = 0; i < listArchivosDescargar.Count; i++)
            {
                streams[i] = new FileStream(listArchivosDescargar[i].Directorio, FileMode.Open, FileAccess.Read);
            }
            PdfDocumentBase.Merge(finalDoc, streams);
            MemoryStream stream = new MemoryStream();
            finalDoc.Save(stream);
            stream.Position = 0;
            finalDoc.Close(true);
            for (int i = 0; i < listArchivosDescargar.Count; i++)
            {
                streams[i].Dispose();
            }
            string contentType = "application/pdf";
            string fileName = $"{nroPedido}.pdf";
            return File(stream, contentType, fileName);
        }

        private bool ExisteDoc(string file)
        {
            return System.IO.File.Exists(file);
        }


    }
}
