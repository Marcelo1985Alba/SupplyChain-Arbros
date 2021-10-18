using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly OrdenesFabricacionController ordenesFabricacionController;
        private readonly CargasController cargasController;

        public ReportRDLCController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvoirement,
            OrdenesFabricacionController ordenesFabricacionController, CargasController cargasController)
        {
            this._context = appDbContext;
            this.webHostEnvoirement = webHostEnvoirement;
            this.ordenesFabricacionController = ordenesFabricacionController;
            this.cargasController = cargasController;
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

        [HttpGet]
        [Route(template: "GetReportEvento")]
        public IActionResult GetReportEvento(int noConf)
        {
            var vale = _context.vEventos.Where(p => p.Cg_NoConf == noConf).ToList();

            var path = $"{webHostEnvoirement.WebRootPath}\\Report\\NoConfor\\ReporteEvento.rdlc";


            Dictionary<string, string> parameter = new();
            parameter.Add("param", "Primer Reporte");

            LocalReport localReport = new(path);

            localReport.AddDataSource(dataSetName: "dsEventos", vale);

            var result = localReport.Execute(RenderType.Pdf, 1, parameter, "");

            return File(result.MainStream, contentType: "application/pdf");
        }

        [HttpGet]
        [Route(template: "GetReportEtiquetaOF")]
        public async Task<IActionResult> GetReportEtiquetaOF(int cg_ordf)
        {
            // Llena tabla de carga

            var orden_fab = await _context.CargaMaq.Where(c => c.CG_ORDFASOC == cg_ordf)
                .OrderByDescending(c=> c.CG_ORDF)
                .FirstOrDefaultAsync();

            string xSQL = "SELECT A.CG_ORDF, A.FE_ENTREGA, A.CG_PROD, A.DES_PROD, A.CG_FORM, " +
                "(rtrim(ltrim(A.PROCESO))) AS PROCESO, rtrim(ltrim(A.CG_CELDA)) CG_CELDA, CG_ORDFORIG, " +
                "(select max(cg_ordf) from programa where  CG_ORDFASOC = A.CG_ORDFASOC) ULTIMAORDENASOCIADA, A.CG_ORDFASOC, " +
                "A.CANT, A.CG_ESTADOCARGA, A.CANTFAB, convert(numeric(6, 2), (A.CANTFAB * 100 / A.CANT)) AS AVANCE, A.DIASFAB, " +
                "(A.DIASFAB * isnull((Select Top 1 ValorN From Solution Where Campo = 'HORASDIA'), 1)) AS HORASFAB, B.EXIGEOA, A.PEDIDO, " +
                "FECHA_PREVISTA_FABRICACION, " +
                "CASE WHEN A.FECHA_INICIO_REAL_FABRICACION is not null THEN A.FECHA_INICIO_REAL_FABRICACION ELSE GETDATE() END FECHA_INICIO_REAL_FABRICACION, " +
                "CASE WHEN A.FE_CIERRE is not null THEN A.FE_CIERRE ELSE GETDATE() END FE_CIERRE, " +
                "A.CG_OPER, A.DES_OPER " +
                "FROM Prod B, Programa A " +
                "LEFT JOIN ProTab ON ProTab.PROCESO = A.PROCESO " +
                "LEFT JOIN Celdas ON Celdas.CG_CELDA = A.CG_CELDA " +
                "WHERE A.CG_PROD = B.CG_PROD AND A.CG_ORDF = " + orden_fab.CG_ORDF;
            var dbOF = await _context.OrdenesFabricacion.FromSqlRaw(xSQL).ToListAsync();
            var path = $"{webHostEnvoirement.WebRootPath}\\Report\\EtiquetaOF\\ReporteEtiquetaOF.rdlc";

            
            Dictionary<string, string> parameter = new();
            parameter.Add("param", "Primer Reporte");

            LocalReport localReport = new(path);
            
            localReport.AddDataSource(dataSetName: "dsEtiquetaOF", dbOF);

            var result = localReport.Execute(RenderType.Pdf, 1, parameter, "");

            return File(result.MainStream, contentType: "application/pdf");
        }
    }
}
