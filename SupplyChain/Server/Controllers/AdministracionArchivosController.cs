using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    }
}
