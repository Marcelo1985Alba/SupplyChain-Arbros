using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class SemaforoController : ControllerBase
    {
        private readonly SemaforoRepository _semforoRepository;

        public SemaforoController(SemaforoRepository semaforoRepository)
        {
            _semforoRepository = semaforoRepository;
        }

            ////[HttpGet("GetSemaforo/{color}")]
            ////public async Task<List<vPresupuestos>> GetSemaforo(string color)
            ////{
            
            ////}

        //[HttpGet]
        //public async Task<List<Semaforo>> GetSemaforo()
        //{
        //    return await _semforoRepository.ObtenerTodosQueryable().Include(c=>c.Valor).ToListAsync();
        //}


       
    }
}
