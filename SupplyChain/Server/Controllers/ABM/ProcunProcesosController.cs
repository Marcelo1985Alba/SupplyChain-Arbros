using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers.ABM
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcunProcesosController : ControllerBase
    {
        //private readonly ProcunProcesosRepository _procesosRepository;
        private readonly AppDbContext _context;

        public ProcunProcesosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcunProceso>>> GetProcunProcesos()
        {
            try
            {
                return await _context.ProcunProceso.ToListAsync();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }

        }

        [HttpGet("{registro}")]
        public async Task<ActionResult<ProcunProceso>> GetProcunProceso(int registro)
        {
            var proceso = await _context.ProcunProceso.FindAsync(registro);
            if (proceso == null)
            {
                return NotFound();
            }
            return proceso;
            //    try
            //    {
            //        var proceso = await _procesosRepository.ObtenerPorId(id);

            //        if (proceso == null)
            //        {
            //            return NotFound();
            //        }

            //        return Ok(proceso);

            //    }
            //    catch (Exception ex)
            //    {
            //        return BadRequest(ex.Message);
            //    }

            //}

        }
    }
}

