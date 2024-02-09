using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.ABM;

namespace SupplyChain.Server.Controllers.ABM
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcedimientosController : ControllerBase
    {
        private readonly ProcedimientosRepository _procedimientosRepository;

        private ProcedimientosController(ProcedimientosRepository procedimientosRepository)
        {
            this._procedimientosRepository = procedimientosRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Procedimiento>>> GetProcedimientos()
        {
            try
            {
                return await _procedimientosRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> GetProcedimientos(string id)
        {
            try
            {
                return await _procedimientosRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //[HttpGet("BuscarProcedimiento/{CG_PROD}/{Busqueda}")]
        //public async Task<ActionResult<List<Procedimientos>>> BuscarProcedimiento(string CG_PROD, int Busqueda)
        //{
        //    List<Procedimientos> procedimientos = new();
        //    if ((string.IsNullOrEmpty(CG_PROD)))
        //    {
        //        procedimientos= (await _procedimientosRepository.ObtenerTodos()).Take(Busqueda).ToList();
        //    }
        //    else if ()
        //    {

        //    }
        //}
    }
}