using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class SemaforoController : ControllerBase
    {
        private readonly SemaforoRepository _semaforoRepository;
        private readonly AppDbContext _context;
        public SemaforoController(SemaforoRepository semaforoRepository)
        {
            _semaforoRepository = semaforoRepository;
        }

        //[HttpGet]
        //public async Task<List<Semaforo>> GetSemaforoTodos()
        //{
        //        return await _semaforoRepository.ObtenerTodos();
        //}

        [HttpGet("{color}")]
        public async Task<Semaforo> GetSemaforo(string color)
        {
            var sem = await _semaforoRepository.Obtener( s=> s.COLOR == color ).FirstOrDefaultAsync();
            
            return sem;
        }



    }
}
