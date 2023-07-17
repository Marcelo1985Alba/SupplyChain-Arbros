using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotivosPresupuestoController : ControllerBase
    {
        private readonly MotivosPresupuestoRepository _motivosPresupuestoRepository;
        private readonly AppDbContext _context;

        public MotivosPresupuestoController(MotivosPresupuestoRepository motivosPresupuestoRepository)
        {
            _motivosPresupuestoRepository = motivosPresupuestoRepository;
        }

        [HttpGet]   
        public async Task<ActionResult<IEnumerable<MotivosPresupuesto>>> GetMotivosTodos()
        {
            try
            {
                var result = await _motivosPresupuestoRepository.ObtenerTodos();
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{motivo}")]
        public async Task<MotivosPresupuesto> GetMotivos(string motivo)
        {
            var motivos = await _motivosPresupuestoRepository.Obtener(m => m.Motivo == motivo).FirstOrDefaultAsync();
            
            return motivos;
        }
    }
}
