using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using Syncfusion.Blazor.RichTextEditor;
using Syncfusion.Pdf.Lists;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers.CDM
{
    [Route("api/[controller]")]
    [ApiController]
    public class cargaValorController : ControllerBase
    {
        private readonly CargaValoresRepository _cargaValoresRepository;

        public cargaValorController(CargaValoresRepository cargaValoresRepository)
        {
            this._cargaValoresRepository = cargaValoresRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Valores>>> GetCargaValores()
        {
            try
            {
                return await _cargaValoresRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        public async Task<ActionResult<IEnumerable<Valores>>> Getcargavalor()
        {
            try
            {
                return await _cargaValoresRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> PutCargaValor(int id, Valores valor)
        {
            if (id != valor.Id)
            {
                return BadRequest();
            }
            try
            {
                await _cargaValoresRepository.Actualizar(valor);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _cargaValoresRepository.Existe(id))
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
            return Ok(valor);
        }

        [HttpPost]
        public async Task<ActionResult<Valores>> PostCargaValor(Valores valor)
        {
            try
            {
                await _cargaValoresRepository.Agregar(valor);
                return CreatedAtAction("GetValor", new { id = valor.Id }, valor);
            }
            catch (DbUpdateException exx)
            {
                if (!await _cargaValoresRepository.Existe(valor.Id))
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
        [HttpDelete("{id}")]
        public async Task<ActionResult<Valores>> DeleteCargaValores(int id)
        {
            var valor = await _cargaValoresRepository.ObtenerPorId(id);
            if (valor == null)
            {
                return NotFound();
            }
            await _cargaValoresRepository.Remover(id);
            return valor;
        }
        [HttpPost("PostList")]

        public async Task<ActionResult<Valores>> PostList(List<Valores> valor)
        {
            try
            {
                foreach (var item in valor)
                {
                    await _cargaValoresRepository.Remover(item.Id);
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
