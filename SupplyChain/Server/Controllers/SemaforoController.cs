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
    public class SemaforoController : ControllerBase
    {
        private readonly SemaforoRepository _semaforoRepository;
        private readonly AppDbContext _context;
        public SemaforoController(SemaforoRepository semaforoRepository)
        {
            _semaforoRepository = semaforoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Semaforo>>> GetSemaforoTodos()
        {
            try { 
            var result = await _semaforoRepository.ObtenerTodos();
            return result;
            }
            catch (Exception ex)
            {
               return BadRequest(ex);
            }
        }

        [HttpGet("{color}")]
        public async Task<Semaforo> GetSemaforo(string color)
        {
            var sem = await _semaforoRepository.Obtener( s=> s.COLOR == color ).FirstOrDefaultAsync();
            
            return sem;
        }



    }
}
