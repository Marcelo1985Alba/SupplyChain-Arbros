using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    public class ControlCalidadController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ControlCalidadController (AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<vControlCalidadPendientes>>> BuscarControl(string CG_ART)
        {
            return await _context.vcontrolCalidadPendientes.ToListAsync();
        }
        
        [HttpGet("byRegistro/{registro}")]
        public async Task<ActionResult<IEnumerable<vControlCalidadPendientes>>> Get (int registro)
        {
            return await _context.vcontrolCalidadPendientes.Where(p=> p.REGISTRO == registro).ToListAsync();
        }
    }
}
