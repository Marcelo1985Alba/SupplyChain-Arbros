﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChain;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Models;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramaController : ControllerBase
    {
        private readonly ProgramaRepository _programaRepository;

        public ProgramaController( ProgramaRepository programaRepository)
        {
            this._programaRepository = programaRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Programa>> Get()
        {
            return await _programaRepository.GetProgramasPedidos();
        }


        [HttpGet("GetPedidos")]
        public async Task<IEnumerable<Programa>> Gets()
        {
            return await _programaRepository.ObtenerTodos();
        }


        // GET: api/Programas/GetPlaneadas
        [HttpGet("GetPlaneadas")]
        public async Task<ActionResult<IEnumerable<Programa>>> GetPlaneadas()
        {
            return await _programaRepository
                .Obtener(p => p.Cg_Cia == 1 && p.CG_ESTADOCARGA == 1 && p.CG_ORDF == p.CG_ORDFASOC, 0 , 
                         r => r.OrderByDescending(p=> p.CG_ORDF))
                .ToListAsync();

        }

        // GET: api/Programas/GetAbastecimientoByOF/73411
        [HttpGet("GetAbastecimientoByOF/{cg_ordf:int}")]
        public async Task<ActionResult<IEnumerable<ItemAbastecimiento>>> GetAbastecimientoByOF(int cg_ordf)
        {
            try
            {
                return Ok(await _programaRepository.GetAbastecimientoByOF(cg_ordf));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // GET: api/Programas/GetProgramaByOF/cg_ordf
        [HttpGet("GetProgramaByOF/{cg_ordf}")]
        public async Task<ActionResult<List<Programa>>> GetCompraByOF(decimal cg_ordf)
        {
            try
            {
                return Ok(await _programaRepository.Obtener(p => p.Cg_Cia == 1
                    && p.CG_ORDF == cg_ordf).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            

        }

        // GET: api/Programas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Programa>> GetPrograma(int id)
        {
            var programa = await _programaRepository.ObtenerPorId(id);

            return programa == null ? NotFound() : programa;
        }

        [HttpGet("EnviarCsvDataCore")]
        public async Task<ActionResult> EnviarCsv()
        {
            await _programaRepository.EnviarCsvDataCore();
            return Ok();
        }

        [HttpGet("GeneraCsvImpresoraQR/{pedido}")]
        public async Task<ActionResult> GeneraCsvImpresoraQR(int pedido)
        {
            //en el sp lo guarda en en c:\temp
            try
            {
                await _programaRepository.GenerarCsvQrByPedido(pedido);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Programas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrograma(int id, Programa programa)
        {
            if (id != programa.REGISTRO)
            {
                return BadRequest();
            }

            try
            {
                await _programaRepository.Actualizar(programa);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await _programaRepository.Existe(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }

            return NoContent();
        }

        // POST: api/Programas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Programa>> PostPrograma(Programa programa)
        {
            await _programaRepository.Agregar(programa);

            return CreatedAtAction("GetPrograma", new { id = programa.REGISTRO }, programa);
        }

        // DELETE: api/Programas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Programa>> DeletePrograma(int id)
        {
            var programa = await _programaRepository.ObtenerPorId(id);
            if (programa == null)
            {
                return NotFound();
            }

            await _programaRepository.Remover(id);

            return programa;
        }

    }
}