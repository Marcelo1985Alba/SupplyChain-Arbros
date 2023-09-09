using System;
using System.Collections.Generic;
using System.IO;
using BoldReports.Web.ReportDesigner;
using BoldReports.Web.ReportViewer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SupplyChain.Server.Controllers;

[Route("api/{controller}/{action}/{id?}")]
[ApiController]
public class ReportDesignerController : ControllerBase, IReportDesignerController
{
    private IMemoryCache _cache;
    private IWebHostEnvironment _hostingEnvironment;

    public ReportDesignerController(IMemoryCache memoryCache, IWebHostEnvironment hostingEnvironment)
    {
        _cache = memoryCache;
        _hostingEnvironment = hostingEnvironment;
    }

    /// <summary>
    ///     Action (HttpGet) method for getting resource of images in the report.
    /// </summary>
    /// <param name="key">The unique key for request identification.</param>
    /// <param name="image">The name of requested image.</param>
    /// <returns>Returns the image as HttpResponseMessage content.</returns>
    public object GetImage(string key, string image)
    {
        return ReportDesignerHelper.GetImage(key, image, this);
    }

    /// <summary>
    ///     Send a GET request and returns the requested resource for a report.
    /// </summary>
    /// <param name="resource">Contains report resource information.</param>
    /// <returns> Resource object for the given key</returns>
    public object GetResource(ReportResource resource)
    {
        return ReportHelper.GetResource(resource, this, _cache);
    }

    /// <summary>
    ///     Report Initialization method that is triggered when report begin processed.
    /// </summary>
    /// <param name="reportOptions">The ReportViewer options.</param>
    [NonAction]
    public void OnInitReportOptions(ReportViewerOptions reportOption)
    {
        //You can update report options here
    }

    /// <summary>
    ///     Report loaded method that is triggered when report and sub report begins to be loaded.
    /// </summary>
    /// <param name="reportOptions">The ReportViewer options.</param>
    [NonAction]
    public void OnReportLoaded(ReportViewerOptions reportOption)
    {
        //You can update report options here
    }

    /// <summary>
    ///     Action (HttpPost) method for posting the request for designer actions.
    /// </summary>
    /// <param name="jsonData">A collection of keys and values to process the designer request.</param>
    /// <returns>Json result for the current request.</returns>
    [HttpPost]
    public object PostDesignerAction([FromBody] Dictionary<string, object> jsonResult)
    {
        return ReportDesignerHelper.ProcessDesigner(jsonResult, this, null, _cache);
    }

    /// <summary>Action (HttpPost) method for posting the request for designer actions.</summary>
    /// <returns>Json result for the current request.</returns>
    public object PostFormDesignerAction()
    {
        return ReportDesignerHelper.ProcessDesigner(null, this, null, _cache);
    }

    public object PostFormReportAction()
    {
        return ReportHelper.ProcessReport(null, this, _cache);
    }

    /// <summary>
    ///     Action (HttpPost) method for posting the request for report process.
    /// </summary>
    /// <param name="jsonResult">The JSON data posted for processing report.</param>
    /// <returns>The object data.</returns>
    [HttpPost]
    public object PostReportAction([FromBody] Dictionary<string, object> jsonResult)
    {
        return ReportHelper.ProcessReport(jsonResult, this, _cache);
    }

    /// <summary>
    ///     Sets the resource into storage location.
    /// </summary>
    /// <param name="key">The unique key for request identification.</param>
    /// <param name="itemId">The unique key to get the required resource.</param>
    /// <param name="itemData">Contains the resource data.</param>
    /// <param name="errorMessage">Returns the error message, if the write action is failed.</param>
    /// <returns>Returns true, if resource is successfully written into storage location.</returns>
    [NonAction]
    bool IReportDesignerController.SetData(string key, string itemId, ItemInfo itemData, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (itemData.Data != null)
        {
            System.IO.File.WriteAllBytes(GetFilePath(itemId, key), itemData.Data);
        }
        else if (itemData.PostedFile != null)
        {
            var fileName = itemId;
            if (string.IsNullOrEmpty(itemId)) fileName = Path.GetFileName(itemData.PostedFile.FileName);

            using (var stream = new MemoryStream())
            {
                itemData.PostedFile.OpenReadStream().CopyTo(stream);
                var bytes = stream.ToArray();
                var writePath = GetFilePath(fileName, key);

                System.IO.File.WriteAllBytes(writePath, bytes);
                stream.Close();
                stream.Dispose();
            }
        }

        return true;
    }

    /// <summary>
    ///     Gets the resource from storage location.
    /// </summary>
    /// <param name="key">The unique key for request identification.</param>
    /// <param name="itemId">The unique key to get the required resource.</param>
    /// <returns>Returns the resource data and error message.</returns>
    [NonAction]
    public ResourceInfo GetData(string key, string itemId)
    {
        var resource = new ResourceInfo();
        try
        {
            var filePath = GetFilePath(itemId, key);
            if (itemId.Equals(Path.GetFileName(filePath), StringComparison.InvariantCultureIgnoreCase) &&
                System.IO.File.Exists(filePath))
                resource.Data = System.IO.File.ReadAllBytes(filePath);
            else
                resource.ErrorMessage = "File not found from the specified path";
        }
        catch (Exception ex)
        {
            resource.ErrorMessage = ex.Message;
        }

        return resource;
    }

    /// <summary>
    ///     Action (HttpPost) method for posted or uploaded file actions.
    /// </summary>
    [HttpPost]
    public void UploadReportAction()
    {
        ReportDesignerHelper.ProcessDesigner(null, this, Request.Form.Files[0], _cache);
    }

    /// <summary>
    ///     Get the path of specific file
    /// </summary>
    /// <param name="itemName">Name of the file to get the full path</param>
    /// <param name="key">The unique key for report designer</param>
    /// <returns>Returns the full path of file</returns>
    [NonAction]
    private string GetFilePath(string itemName, string key)
    {
        var dirPath = Path.Combine(_hostingEnvironment.WebRootPath + "\\" + "Cache", key);

        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        return Path.Combine(dirPath, itemName);
    }
}