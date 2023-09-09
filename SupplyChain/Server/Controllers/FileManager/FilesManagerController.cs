using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SupplyChain.Server.Repositorios;
using Syncfusion.Blazor.FileManager;
using Syncfusion.Blazor.FileManager.Base;
using FileManagerDirectoryContent = Syncfusion.Blazor.FileManager.Base.FileManagerDirectoryContent;
//File Manager's base functions are available in the below namespace
//File Manager's operations are available in the below namespace

namespace filemanager.Server.Controllers;

[Route("api/[controller]")]
public class FilesManagerController : Controller
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly SolutionRepository _solutionRepository;
    public string basePath;
    public PhysicalFileProvider operation;
    private readonly string root = "";

    public FilesManagerController(SolutionRepository solutionRepository,
        AuthenticationStateProvider authenticationStateProvider)
    {
        operation = new PhysicalFileProvider();
        _solutionRepository = solutionRepository;
        root = solutionRepository.Obtener(s => s.CAMPO == "RUTADOCS").FirstOrDefault().VALORC;
        operation.RootFolder(root); // It denotes in which files and folders are available.
        //this._authenticationStateProvider = authenticationStateProvider;
        operation.SetRules(GetRules());
    }

    // Processing the File Manager operations
    [Route("FileOperations")]
    public object FileOperations([FromBody] FileManagerDirectoryContent args)
    {
        try
        {
            switch (args.Action)
            {
                // Add your custom action here
                case "read":
                    // Path - Current path; ShowHiddenItems - Boolean value to show/hide hidden items
                    return operation.ToCamelCase(operation.GetFiles(args.Path, args.ShowHiddenItems));
                case "delete":
                    // Path - Current path where of the folder to be deleted; Names - Name of the files to be deleted
                    return operation.ToCamelCase(operation.Delete(args.Path, args.Names));
                case "copy":
                    //  Path - Path from where the file was copied; TargetPath - Path where the file/folder is to be copied; RenameFiles - Files with same name in the copied location that is confirmed for renaming; TargetData - Data of the copied file
                    return operation.ToCamelCase(operation.Copy(args.Path, args.TargetPath, args.Names,
                        args.RenameFiles, args.TargetData));
                case "move":
                    // Path - Path from where the file was cut; TargetPath - Path where the file/folder is to be moved; RenameFiles - Files with same name in the moved location that is confirmed for renaming; TargetData - Data of the moved file
                    return operation.ToCamelCase(operation.Move(args.Path, args.TargetPath, args.Names,
                        args.RenameFiles, args.TargetData));
                case "details":
                    // Path - Current path where details of file/folder is requested; Name - Names of the requested folders
                    return operation.ToCamelCase(operation.Details(args.Path, args.Names));
                case "create":
                    // Path - Current path where the folder is to be created; Name - Name of the new folder
                    var create = operation.Create(args.Path, args.Name);
                    return operation.ToCamelCase(operation.Create(args.Path, args.Name));
                case "search":
                    // Path - Current path where the search is performed; SearchString - String typed in the searchbox; CaseSensitive - Boolean value which specifies whether the search must be casesensitive
                    return operation.ToCamelCase(operation.Search(args.Path, args.SearchString,
                        args.ShowHiddenItems, args.CaseSensitive));
                case "rename":
                    // Path - Current path of the renamed file; Name - Old file name; NewName - New file name
                    return operation.ToCamelCase(operation.Rename(args.Path, args.Name, args.NewName));
            }

            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    // uploads the file(s) into a specified path
    [Route("Upload")]
    public IActionResult Upload(string path, IList<IFormFile> uploadFiles, string action)
    {
        try
        {
            FileManagerResponse uploadResponse;
            uploadResponse = operation.Upload(path, uploadFiles, action, null);
            if (uploadResponse.Error != null)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = Convert.ToInt32(uploadResponse.Error.Code);
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase =
                    uploadResponse.Error.Message;
            }

            return Content("");
        }
        catch (Exception ex)
        {
            return Content("");
        }
    }

    // downloads the selected file(s) and folder(s)
    [Route("Download")]
    public IActionResult Download(string downloadInput)
    {
        var args =
            JsonConvert.DeserializeObject<FileManagerDirectoryContent>(
                downloadInput);
        return operation.Download(args.Path, args.Names, args.Data);
    }

    // gets the image(s) from the given path
    [Route("GetImage")]
    public IActionResult GetImage(FileManagerDirectoryContent args)
    {
        return operation.GetImage(args.Path, args.Id, false, null, null);
    }

    protected AccessDetails GetRules()
    {
        AccessDetails accessDetails = new();
        //var result = _authenticationStateProvider.GetAuthenticationStateAsync();
        //result.Wait();

        //var user = result.Result;
        accessDetails.AccessRules = (List<AccessRule>)new List<AccessRule>
        {
            new AccessRule
            {
                Path = "/*.*",
                Read = Permission.Allow,
                Write = Permission.Allow,
                Copy = Permission.Allow,
                WriteContents = Permission.Allow,
                Upload = Permission.Allow,
                Download = Permission.Allow,
                IsFile = true
            },

            new AccessRule
            {
                Path = "/*.*",
                Read = Permission.Allow,
                Write = Permission.Allow,
                Copy = Permission.Allow,
                WriteContents = Permission.Allow, Upload = Permission.Allow, Download = Permission.Allow,
                IsFile = false
            }
        };

        return accessDetails;
    }
}