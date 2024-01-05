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
        private readonly ProcunProcesosRepository _procesosRepository;

        public ProcunProcesosController(ProcunProcesosRepository ProcunProcesosRepository)
        {
            this._procesosRepository = ProcunProcesosRepository;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcunProcesos>>> GetProcunProcesos()
        {
            try
            {var respuesta = await _procesosRepository.ObtenerTodos();
                return respuesta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProcunProcesos>> GetProcunProceso(int id)
        {
            try
            {
                var proceso = await _procesosRepository.ObtenerPorId(id);

                if (proceso == null)
                {
                    return NotFound();
                }

                return Ok(proceso);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
