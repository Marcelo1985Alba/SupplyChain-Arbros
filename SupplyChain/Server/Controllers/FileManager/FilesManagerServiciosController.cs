using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
//File Manager's base functions are available in the below namespace
using Syncfusion.Blazor.FileManager.Base;
//File Manager's operations are available in the below namespace
using Syncfusion.Blazor.FileManager;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Server.Repositorios;
using Microsoft.AspNetCore.Components.Authorization;

namespace filemanager.Server.Controllers
{
    [Route("api/[controller]")]
    public class FilesManagerServiciosController : Controller
    {
        private readonly SolutionRepository _solutionRepository;
        public PhysicalFileProvider operation;
        public string basePath;
        string root = "";
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public FilesManagerServiciosController(SolutionRepository solutionRepository, AuthenticationStateProvider authenticationStateProvider )
        {
            this.operation = new PhysicalFileProvider();
            this._solutionRepository = solutionRepository;
            this.root = solutionRepository.Obtener(s => s.CAMPO == "RUTAFOTOSSOL").FirstOrDefault().VALORC;
            this.operation.RootFolder(this.root); // It denotes in which files and folders are available.
            this._authenticationStateProvider = authenticationStateProvider;
            
        }

        // Processing the File Manager operations
        [Route("FileOperations")]
        public object FileOperations([FromBody] Syncfusion.Blazor.FileManager.Base.FileManagerDirectoryContent args)
        {
            
            this.operation.SetRules(GetRules());
            switch (args.Action)
            {
                // Add your custom action here
                case "read":
                    // Path - Current path; ShowHiddenItems - Boolean value to show/hide hidden items
                    return this.operation.ToCamelCase(this.operation.GetFiles(args.Path, args.ShowHiddenItems));
                case "delete":
                    // Path - Current path where of the folder to be deleted; Names - Name of the files to be deleted
                    return this.operation.ToCamelCase(this.operation.Delete(args.Path, args.Names));
                case "copy":
                    //  Path - Path from where the file was copied; TargetPath - Path where the file/folder is to be copied; RenameFiles - Files with same name in the copied location that is confirmed for renaming; TargetData - Data of the copied file
                    return this.operation.ToCamelCase(this.operation.Copy(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData));
                case "move":
                    // Path - Path from where the file was cut; TargetPath - Path where the file/folder is to be moved; RenameFiles - Files with same name in the moved location that is confirmed for renaming; TargetData - Data of the moved file
                    return this.operation.ToCamelCase(this.operation.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData));
                case "details":
                    // Path - Current path where details of file/folder is requested; Name - Names of the requested folders
                    return this.operation.ToCamelCase(this.operation.Details(args.Path, args.Names));
                case "create":
                    // Path - Current path where the folder is to be created; Name - Name of the new folder
                    return this.operation.ToCamelCase(this.operation.Create(args.Path, args.Name));
                case "search":
                    // Path - Current path where the search is performed; SearchString - String typed in the searchbox; CaseSensitive - Boolean value which specifies whether the search must be casesensitive
                    return this.operation.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive));
                case "rename":
                    // Path - Current path of the renamed file; Name - Old file name; NewName - New file name
                    return this.operation.ToCamelCase(this.operation.Rename(args.Path, args.Name, args.NewName));
            }
            return null;
        }

        // uploads the file(s) into a specified path
        [Route("Upload")]
        public IActionResult Upload(string path, IList<IFormFile> uploadFiles, string action)
        {
            FileManagerResponse uploadResponse;
            uploadResponse = operation.Upload(path, uploadFiles, action, null);
            if (uploadResponse.Error != null)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = Convert.ToInt32(uploadResponse.Error.Code);
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = uploadResponse.Error.Message;
            }
            return Content("");
        }

        // downloads the selected file(s) and folder(s)
        [Route("Download")]
        public IActionResult Download(string downloadInput)
        {
            Syncfusion.Blazor.FileManager.Base.FileManagerDirectoryContent args = JsonConvert.DeserializeObject<Syncfusion.Blazor.FileManager.Base.FileManagerDirectoryContent>(downloadInput);
            return operation.Download(args.Path, args.Names, args.Data);
        }

        // gets the image(s) from the given path
        [Route("GetImage")]
        public IActionResult GetImage(Syncfusion.Blazor.FileManager.Base.FileManagerDirectoryContent args)
        {
            return this.operation.GetImage(args.Path, args.Id, false, null, null);
        }

        protected AccessDetails GetRules()
        {
            AccessDetails accessDetails = new();
            //var result = _authenticationStateProvider.GetAuthenticationStateAsync();
            //result.Wait();
            var userName = HttpContext.User.Identity.Name;
            var AllowCopyWrite = HttpContext.User.Claims.Any(c => c.Value == "ArchivosServicios" || c.Value == "Administrador");
            var permiteCopyWhrite = AllowCopyWrite ? Permission.Allow : Permission.Deny;
            accessDetails.AccessRules = (List<AccessRule>)(new()
            {
                new AccessRule
                {
                    Path = "/*.*",
                    Read = Permission.Allow,
                    Write = permiteCopyWhrite,
                    Copy = permiteCopyWhrite,
                    WriteContents = permiteCopyWhrite,
                    Upload = Permission.Allow,
                    Download = Permission.Allow,
                    IsFile = true, 
                },

                new AccessRule { Path = "/*.*",
                    Read = Permission.Allow,
                    Write = Permission.Allow,
                    Copy = permiteCopyWhrite,
                    WriteContents = Permission.Allow, Upload = Permission.Allow, Download = Permission.Allow, IsFile = false },
            });

            return accessDetails;
        }
    }
}