using System;
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
        private readonly AppDbContext _context;
        private readonly CompraRepository _compraRepository;
        private readonly PedidosRepository _pedidosRepository;
        private readonly ProveedorRepository _proveedorRepository;
        private readonly GeneraRepository _generaRepository;

        public ComprasController( AppDbContext context,
            CompraRepository compraRepository, PedidosRepository pedidosRepository,
                                 ProveedorRepository proveedorRepository, GeneraRepository generaRepository)
        {
            this._context = context;
            this._compraRepository = compraRepository;
            this._pedidosRepository = pedidosRepository;
            this._proveedorRepository = proveedorRepository;
            this._generaRepository = generaRepository;
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


        /**/
        // GET: api/Compras/GetPreparacion
        [HttpGet("GetPreparacion/{cgprove}")]
        public async Task<ActionResult<IEnumerable<Compra>>> GetPreparacion(int cgprove)
        {
            try
            {
                if (cgprove > 0)
                {
                    return Ok(await _compraRepository.Obtener(c => c.NUMERO == 0 && c.NROCLTE == cgprove).ToListAsync());
                }
                else
                {
                    return Ok(await _compraRepository.Obtener(c => c.NUMERO == 0 && c.NROCLTE > 0).ToListAsync());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Compras/GetSugerencia
        [HttpGet("GetSugerencia")]
        public async Task<ActionResult<IEnumerable<Compra>>> GetSugerencia()
        {
            try
            {
                return Ok(await _compraRepository.Obtener(c => c.NUMERO == 0 && c.NROCLTE == 0).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost("agregaitem")]
        //POST: api/compras/agregaitem
        public async Task<ActionResult<Compra>> PostStock([FromBody] Compra Itemcompras)
        {

            try
            {
                await _compraRepository.Agregar(Itemcompras);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            return Ok(Itemcompras);
        }

        [HttpPut("actualizaitem/{registro}")]
        //POST: api/compras/actualizaitem/5
        public async Task<ActionResult<Compra>> PutStock(int registro, Compra Itemcompras)
        {
            if (registro != Itemcompras.Id)
            {
                return BadRequest("Registro Incorrecto");
            }

            try
            {
                await _compraRepository.Actualizar(Itemcompras);
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                BadRequest(dbEx);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            return Ok(Itemcompras);
        }

        // GET: api/Compras/GetProveedorescompras
        [HttpGet("GetProveedorescompras")]
        public async Task<ActionResult<IEnumerable<Proveedores_compras>>> GetProveedorescompras()
        {
            string xSQL = string.Format("SELECT NROCLTE, DES_PROVE FROM COMPRAS WHERE NROCLTE > 0 AND NUMERO = 0" +
                " AND PRECIO > 0 group by NROCLTE, DES_PROVE");
            return await _context.proveedores_compras.FromSqlRaw(xSQL).ToListAsync();
        }

        // GET: api/Compras/Ultimascompras/cgmat
        [HttpGet("Ultimascompras/{cgmat}")]
        public async Task<ActionResult<IEnumerable<Compra>>> Ultimascompras(string cgmat)
        {
            return Ok( await _compraRepository.UltimasCompras(3, cgmat));
        }

        [HttpPut("actualizaoc/{listaordenescompra}")]
        //POST: api/compras/generaoc
        public async Task<ActionResult<Compra>> actualizaoc(string listaordenescompra)
        {
            var numoc = (int)0;

            try
            {
                //RESERVA REGISTRO: El vale hay que hacerlo del lado del cliente porque debe reservar un solo vale
                //y aqui se ejecuta por item.
                await _generaRepository.Reserva("NUMERO");
                var genera = _generaRepository.Obtener(g => g.Id == "NUMERO").FirstOrDefault();
                numoc = (int)genera.VALOR1;

                string sqlCommandString = string.Format("UPDATE COMPRAS SET NUMERO = " + numoc + " " +
                    "WHERE REGISTRO IN (" + listaordenescompra.Remove(listaordenescompra.Length - 1) + ")");
                await _generaRepository.Database.ExecuteSqlRawAsync(sqlCommandString);

                await _generaRepository.Libera("NUMERO");
            }
            catch (Exception e)
            {
                await _generaRepository.Libera("NUMERO");
                return BadRequest(e);
            }
            return Ok(numoc);
        }

    }
}
