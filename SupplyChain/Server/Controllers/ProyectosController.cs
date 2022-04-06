using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProyectosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/GetProyectos
        [HttpGet("Proyectos")]
        public object GetProyectos()
        {
            IQueryable<ProyectosGBPI> data = _context.Proyectos.AsQueryable();
            return new { Items = data, Count = data.Count() };
        }
    }
}