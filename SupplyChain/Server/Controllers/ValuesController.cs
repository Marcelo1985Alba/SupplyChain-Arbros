using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Syncfusion.Blazor.PdfViewer;

namespace SupplyChain
{

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        //Initialize the memory cache object   
        public IMemoryCache _cache;
        private readonly AppDbContext _context;
        private readonly ILogger<ValuesController> logger;

        public ValuesController(IWebHostEnvironment hostingEnvironment, IMemoryCache cache
            , AppDbContext appContext
            , ILogger<ValuesController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
            this._context = appContext;
            this.logger = logger;
        }

        [HttpPost("Load")]
        //
        [Route("[controller]/Load")]
        //Post action for Loading the PDF documents   
        public IActionResult Load([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            MemoryStream stream = new MemoryStream();
            object jsonResult = new object();
            if (jsonObject != null && jsonObject.ContainsKey("document"))
            {
                if (bool.Parse(jsonObject["isFileName"]))
                {
                    string documentPath = GetDocumentPath(jsonObject["document"]);
                    if (!string.IsNullOrEmpty(documentPath))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(documentPath);
                        stream = new MemoryStream(bytes);
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


        //[AcceptVerbs("Post")]
        //[HttpPost("Image")]
        //
        //[Route("[controller]/Image")]
        ////Post action for processing the bookmarks from the PDF documents
        //public byte[] Image()
        //{
        //    string filePath = @"S:\documents\Capture.jpeg";
        //    using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            fileStream.CopyTo(memoryStream);
        //            Bitmap image = new Bitmap(1, 1);
        //            image.Save(memoryStream, ImageFormat.Jpeg);
        //            byte[] byteImage = memoryStream.ToArray();
        //            return byteImage;
        //        }
        //    }
        //}

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
        [HttpPost("RenderPdfPages")]

        [Route("[controller]/RenderPdfPages")]
        //Post action for processing the PDF documents  
        public IActionResult RenderPdfPages([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetPage(jsonObject);
            var result = JsonConvert.SerializeObject(jsonResult);

            //var bytes = Encoding.UTF8.GetBytes(jsonResult.ToString());
            //return File(jsonResult, "application/octet-stream");
            return Content(result);
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
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
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
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = string.Empty;
            if (jsonObject != null && jsonObject.ContainsKey("fileName"))
            {
                string documentPath = GetDocumentPath(jsonObject["fileName"]);
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
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
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

        //Gets the path of the PDF document
        private string GetDocumentPath(string document)
        {

            var contenido = document.Split(',');

            document = contenido[0];

            var ubicacion =  _context.Solution.Where(s=> s.CAMPO == contenido[1]).Select(c=> c.VALORC).FirstOrDefault();
            string documentPath = string.Empty;

            if (!System.IO.File.Exists(ubicacion + "/" + document))
            {
                //var sol = ubicaciones.Where(s => s.CAMPO == contenido[1]).FirstOrDefault();
                //var path = _hostingEnvironment.ContentRootPath;
                //string webRootPath = _hostingEnvironment.WebRootPath;
                //if (System.IO.File.Exists(path + "/documentos/" + document))
                //    documentPath = path + "/documentos/" + document;
                ubicacion = "D:\\Descargas\\";
                document = "27303156459_011_00001_00000058.pdf";
                if (System.IO.File.Exists(ubicacion +"/" + document))
                    documentPath = ubicacion + "/" + document;
            }
            else
            {
                documentPath = ubicacion + "/" + document;
            }
            Console.WriteLine(documentPath);
            return documentPath;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            int size = IntPtr.Size;
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {

            return "value";
        }
    }
}