using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ProveedoresController(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Proveedor>> Get()
        {
            return await _appDbContext.Proveedores.ToListAsync();
        }

        [HttpGet("id")]
        public async Task<IEnumerable<Proveedor>> ById(int id)
        {
            return await _appDbContext.Proveedores.Where(p=> p.Id == id).ToListAsync();
        }
    }
}
