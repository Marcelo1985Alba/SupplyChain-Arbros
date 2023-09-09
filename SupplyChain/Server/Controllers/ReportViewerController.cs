using System.Collections.Generic;
using System.IO;
using BoldReports.Web.ReportViewer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SupplyChain.Server.Controllers;

[Route("api/{controller}/{action}/{id?}")]
[ApiController]
public class ReportViewerController : ControllerBase, IReportController
{
    // Report viewer requires a memory cache to store the information of consecutive client request and
    // have the rendered report viewer information in server.
    private IMemoryCache _cache;

    // IHostingEnvironment used with sample to get the application data from wwwroot.
    private IHostingEnvironment _hostingEnvironment;

    // Post action to process the report from server based json parameters and send the result back to the client.
    public ReportViewerController(IMemoryCache memoryCache,
        IHostingEnvironment hostingEnvironment)
    {
        _cache = memoryCache;
        _hostingEnvironment = hostingEnvironment;
    }

    // Post action to process the report from server based json parameters and send the result back to the client.
    [HttpPost]
    public object PostReportAction([FromBody] Dictionary<string, object> jsonArray)
    {
        return ReportHelper.ProcessReport(jsonArray, this, _cache);
    }

    // Method will be called to initialize the report information to load the report with ReportHelper for processing.
    public void OnInitReportOptions(ReportViewerOptions reportOption)
    {
        var basePath = _hostingEnvironment.WebRootPath;
        var inputStream = new FileStream(basePath + @"\Report\" + reportOption.ReportModel.ReportPath + ".rdlc",
            FileMode.Open, FileAccess.Read);
        reportOption.ReportModel.Stream = inputStream;
    }

    // Method will be called when reported is loaded with internally to start to layout process with ReportHelper.
    public void OnReportLoaded(ReportViewerOptions reportOption)
    {
    }

    //Get action for getting resources from the report
    [ActionName("GetResource")]
    [AcceptVerbs("GET")]
    // Method will be called from Report Viewer client to get the image src for Image report item.
    public object GetResource(ReportResource resource)
    {
        return ReportHelper.GetResource(resource, this, _cache);
    }

    [HttpPost]
    public object PostFormReportAction()
    {
        return ReportHelper.ProcessReport(null, this, _cache);
    }
}