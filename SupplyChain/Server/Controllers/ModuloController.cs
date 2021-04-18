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
    public class ModuloController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public ModuloController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Modulo>>> Get()
        //{
        //    try
        //    {
        //        return await appDbContext.Modulos.ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }
        //}

        [HttpGet]
        public object GetSyncfusion()
        {
            //Get the DataSource from Database
            var data = appDbContext.Modulos.ToList();
            var queryString = Request.Query;
            if (queryString.Keys.Contains("$filter"))
            {
                StringValues Skip;
                StringValues Take;
                int skip = (queryString.TryGetValue("$skip", out Skip)) ? Convert.ToInt32(Skip[0]) : 0;
                int top = (queryString.TryGetValue("$top", out Take)) ? Convert.ToInt32(Take[0]) : data.Count();
                string filter = string.Join("", queryString["$filter"].ToString().Split(' ').Skip(2)); // get filter from querystring
                data = data.Where(d => d.ParentId.ToString() == filter).ToList();
                return data.Skip(skip).Take(top);
            }
            else
            {
                data = data.Where(d => d.ParentId == null).ToList();
                return data;
            }
        }

        [HttpGet("{id}")]
        public object GetIndex(string id)
        {
            // Get the DataSource from Database
            var data = appDbContext.Modulos.ToList();
            int index;
            var count = data.Count;
            if (count > 0)
            {
                index = (data[data.Count - 1].Id);
            }
            else
            {
                index = 0;
            }
            return index;
        }

        [HttpPost]
        public void Post([FromBody] Modulo modulo)
        {
            appDbContext.Add(modulo);
        }
        [HttpPut]
        public object Put([FromBody] Modulo modulo)
        {
            appDbContext.Add(modulo);
            return modulo;
        }
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            appDbContext.Modulos.ToList();
        }


    }
}
