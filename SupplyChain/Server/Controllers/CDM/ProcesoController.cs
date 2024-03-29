﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using Syncfusion.Blazor.RichTextEditor;
using Syncfusion.Pdf.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


//Utiliza el model pROCESOS // 
//public class cargaValorController llamar ProcesoController 
//llamar al archivo Proceso

namespace SupplyChain.Server.Controllers.CDM
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesoController : ControllerBase
    {
        private readonly ProcesoRepository _procesoRepository;

        public ProcesoController(ProcesoRepository procesoRepository)
        {
            this._procesoRepository= procesoRepository;
        }

        // GET: api/Valores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Procesos>>> GetCargaValores(string VALE)
        {
            try
            {
                return await _procesoRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Proceso/BuscarPorVALE/{VALE}
        [HttpGet("BuscarPorVALE/{VALE}")]
        public async Task<ActionResult<IEnumerable<Procesos>>> BuscarPorVale(string VALE)
        {
            try
            {
                return await _procesoRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Proceso/BuscarPorDESPACHO/{DESPACHO}
        [HttpGet("BuscarPorDESPACHO/{DESPACHO}")]
        public async Task<ActionResult<List<Procesos>>> BuscarPorDESPACHO(string DESPACHO)
        {
            List<Procesos> lProcesos = new List<Procesos>();
            if (_procesoRepository.ObtenerTodosQueryable().Any())
            {
                lProcesos = await _procesoRepository.Obtener(p => p.DESPACHO == DESPACHO).ToListAsync();
            }
            if (lProcesos == null)
            {
                return NotFound();
            }
            return lProcesos;
        }

        //GET: api/Valores/Existe/{id}
        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteValor(int id)
        {
            try
            {
                return await _procesoRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //GET: api/Valores/Existe/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> PutCargaValor(int id, Procesos valor)
        {
            if (id != valor.Id)
            {
                return BadRequest();
            }
            try
            {
                await _procesoRepository.Actualizar(valor);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _procesoRepository.Existe(id))
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


        //POST: api/Valores
        [HttpPost]
        public async Task<ActionResult<Procesos>> PostCargaValor(Procesos valor)
        {
            try
            {
                await _procesoRepository.Agregar(valor);
                return CreatedAtAction("GetCargaValores", new { id = valor.Id }, valor);
            }
            catch (DbUpdateException exx)
            {
                if (!await _procesoRepository.Existe(valor.Id))
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

        //DELETE: api/Valores/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Procesos>> DeleteCargaValores(int id)
        {
            var valor = await _procesoRepository.ObtenerPorId(id);
            if (valor == null)
            {
                return NotFound();
            }
            await _procesoRepository.Remover(id);
            return valor;
        }

        //POST: api/Valores/PostList
        [HttpPost("PostList")]
        public async Task<ActionResult<List<Procesos>>> PostList([FromBody]List<Procesos> valor)
        {
            try
            {
                foreach (var item in valor)
                {
                    await _procesoRepository.Agregar(item);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(valor);
        }
    }
}
