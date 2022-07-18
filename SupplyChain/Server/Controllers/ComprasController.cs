﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly int cg_cia_usuario = 1; /*CAMBIAR POR LA DEL USUARIO*/
        private readonly CompraRepository _compraRepository;
        private readonly PedidosRepository _pedidosRepository;
        private readonly ProveedorRepository _proveedorRepository;

        public ComprasController(CompraRepository compraRepository, PedidosRepository pedidosRepository,
                                 ProveedorRepository proveedorRepository)
        {
            this._compraRepository = compraRepository;
            this._pedidosRepository = pedidosRepository;
            this._proveedorRepository = proveedorRepository;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompras()
        {
            //OC ABIERTAS
            try
            {
                var compras = await _compraRepository
                    .Obtener(c => c.CG_CIA == cg_cia_usuario && c.FE_CIERRE == null && c.NUMERO > 0).ToListAsync();

                await compras.ForEachAsync( async c =>
                {
                    c.PENDIENTE = c.SOLICITADO - await _pedidosRepository.ObtenerRecepSumByOcMp(c.NUMERO, c.CG_MAT);
                    c.ProveedorNavigation = await _proveedorRepository.ObtenerPorId(c.NROCLTE);
                });

                return compras.OrderByDescending(c=> c.NUMERO).ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> GetCompra(int id)
        {
            var compra = await _compraRepository.ObtenerPorId(id);

            if (compra == null)
            {
                return NotFound();
            }

            return Ok(compra);
        }

        // GET: api/Compras/GetCompraByNumero/5
        [HttpGet("GetCompraByNumero/{numero}")]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompraByNumero(int numero)
        {
            try
            {
               return Ok(await _compraRepository.Obtener(c=> c.NUMERO == numero).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Compras/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompra(decimal id, Compra compra)
        {
            if (id != compra.Id)
            {
                return BadRequest();
            }

            try
            {
                await _compraRepository.Actualizar(compra);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await _compraRepository.Existe(id))
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

        // POST: api/Compras
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Compra>> PostCompra(Compra compra)
        {
            await _compraRepository.Agregar(compra);

            return CreatedAtAction("GetCompra", new { id = compra.Id }, compra);
        }

        // DELETE: api/Compras/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Compra>> DeleteCompra(int id)
        {
            var compra = await _compraRepository.ObtenerPorId(id);
            if (compra == null)
            {
                return NotFound();
            }

            await _compraRepository.Remover(id);

            return compra;
        }

        
    }
}
