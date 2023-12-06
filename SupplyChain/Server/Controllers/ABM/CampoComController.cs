using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.Prod;


namespace SupplyChain.Server.Controllers.ABM
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampoComController : ControllerBase
    {
        private readonly CampoComRepository _campoRepository;

        public CampoComController(CampoComRepository campoComRepository)
        {
            this._campoRepository= campoComRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampoComodin>>> GetCampo()
        {
            try
            {
                return await _campoRepository.ObtenerTodos();
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost]
        public async Task<ActionResult<CampoComodin>> PostCampos(CampoComodin campo)
        {
            try
            {
                    await _campoRepository.Agregar(campo);
                    return CreatedAtAction("GetCampo", new { id = campo.Id }, campo);
            }catch(DbUpdateException ex)
            {
                if(!await _campoRepository.Existe(campo.Id))
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

        [HttpPost("PostList")]
        public async Task<ActionResult<CampoComodin>> PostList(List<CampoComodin> campo)
        {
            try
            {
                foreach(var item in campo)
                {
                    await _campoRepository.Remover(item.Id);
                }
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampos(int id, CampoComodin campo)
        {
            if (id != campo.Id)
            {
                return BadRequest();
            }
            try
            {
                await _campoRepository.Actualizar(campo);
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!await _campoRepository.Existe(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok(campo);
        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteCampo(int id)
        {
            try
            {
                return await _campoRepository.Existe(id);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<CampoComodin>> DeleteCampos(int id)
        {
            var campo = await _campoRepository.ObtenerPorId(id);
            if (campo == null)
            {
                return NotFound();  
            }

            await _campoRepository.Remover(id);
            return campo;
        }



    }
}
