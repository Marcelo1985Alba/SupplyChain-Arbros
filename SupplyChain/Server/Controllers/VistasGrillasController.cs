using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VistasGrillasController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public VistasGrillasController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        [HttpGet("GetByName/{name}")]
        public async Task<ActionResult<List<VistasGrillas>>> GetByName(string name)
        {
            return await appDbContext.VistasGrillas.Where(v => v.AppName == name).ToListAsync();
        }

        [HttpGet("GetByNameSyncf/{name}")]
        public object GetByNameSyncf(string name)
        {
            IQueryable<VistasGrillas> data = appDbContext.VistasGrillas.Where(v => v.AppName == name).AsQueryable();
            var count = data.Count();
            var queryString = Request.Query;
            if (queryString.Keys.Contains("$inlinecount"))
            {
                StringValues Skip;
                StringValues Take;
                int skip = (queryString.TryGetValue("$skip", out Skip)) ? Convert.ToInt32(Skip[0]) : 0;
                int top = (queryString.TryGetValue("$top", out Take)) ? Convert.ToInt32(Take[0]) : data.Count();
                return new { Items = data.Skip(skip).Take(top), Count = count };
            }
            else
            {
                return data;
            }
        }
    }
}
