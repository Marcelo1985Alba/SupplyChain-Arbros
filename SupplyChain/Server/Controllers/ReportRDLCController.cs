using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportRDLCController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment webHostEnvoirement;

        public ReportRDLCController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvoirement)
        {
            this._context = appDbContext;
            this.webHostEnvoirement = webHostEnvoirement;
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [HttpGet]
        [Route(template: "GetReport")]
        public IActionResult GetReport(int numeroVale)
        {
            var vale =_context.Pedidos.Where(p => p.VALE == numeroVale).ToList();
             
            var path = $"{webHostEnvoirement.WebRootPath}\\Report\\Report1.rdlc";


            Dictionary<string, string> parameter = new();
            parameter.Add("param", "Primer Reporte");

            LocalReport localReport = new(path);
            
            localReport.AddDataSource(dataSetName: "dsPedidos", vale);

            var result = localReport.Execute(RenderType.Pdf, 1, parameter, "");

            return File(result.MainStream, contentType: "application/pdf");
        }
    }
}
