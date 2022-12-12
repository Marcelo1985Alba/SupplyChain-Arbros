using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using Syncfusion.Blazor.RichTextEditor;
using Syncfusion.Pdf.Lists;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers.ABM
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProMPController : ControllerBase
    { 
       
        private readonly ProcalMPRepository _procalMPRepository;

        public ProMPController(ProcalMPRepository procalMPRepository)
        {
            this._procalMPRepository = procalMPRepository;
        }

        // GET: api/ProcalsMP
        [HttpGet]
        //cambiar nombre del Get GetProcalMP()
        public async Task<ActionResult<IEnumerable<ProcalsMP>>> GetProcalMP()
        {
            try
            {
                return await _procalMPRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        public async Task<ActionResult<IEnumerable<ProcalsMP>>> GetProMPS()
        {
            try
            {
                return await _procalMPRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //PUTC: api/ProcalMP/{id}
        [HttpPut("{id}")]
        
        public async Task<ActionResult> PutProcalMP(int id, ProcalsMP procalMP)
        {

            if(id !=procalMP.Id)
            {
                return BadRequest();
            }
            try
            {
                await _procalMPRepository.Actualizar(procalMP);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _procalMPRepository.Existe(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok(procalMP);
        }

        // POST: api/ProcalsMP
        [HttpPost]
        public async Task<ActionResult<ProcalsMP>> PostProcalMP(ProcalsMP procalMP)
        {

            try
            {
                await _procalMPRepository.Agregar(procalMP);
                return CreatedAtAction("GetProcalsMP", new { id = procalMP.Id }, procalMP);
            }
            catch (DbUpdateException exx)
            {
                if(!await _procalMPRepository.Existe(procalMP.Id))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //DELETE :api/ProcalsMP/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProcalsMP>> DeteleProcalsMP(int id)
        {
            var procalMP = await _procalMPRepository.ObtenerPorId(id);
            if(procalMP == null)
            {
                return NotFound();
            }
            await _procalMPRepository.Remover(id);
            return procalMP;
        }
        //POST :api/ProcalsMP/PostList
        [HttpPost("PostList")]

        public async Task<ActionResult<ProcalsMP>> PostList(List<ProcalsMP> procalsMP)
        {
            try
            {
                foreach(var item in procalsMP)
                {
                    await _procalMPRepository.Remover(item.Id);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }

    }
}
       
