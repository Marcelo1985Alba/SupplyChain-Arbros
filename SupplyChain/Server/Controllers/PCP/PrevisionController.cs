using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChain;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrevisionController : ControllerBase
    {
        private readonly PrevisionRepository _previsionRepository;
        private readonly ProductoRepository _productoRepository;

        public PrevisionController(PrevisionRepository previsionRepository, ProductoRepository productoRepository)
        {
            this._previsionRepository = previsionRepository;
            this._productoRepository = productoRepository;
        }

        // GET: api/Prevision
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PresAnual>>> GetPrev()
        {
            try
            {
                return await _previsionRepository.ObtenerTodos();
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        //// GET: api/Prevision/GetProd
        //[HttpGet("GetProd")]
        //public IEnumerable<Producto> GetProd(string PEDIDO)
        //{
        //    string xSQL = string.Format("SELECT * FROM Prod ");
        //    return _context.Prod.FromSqlRaw(xSQL).ToList<Producto>();
        //}

        //// GET: api/Prevision/BuscarPorCG_PROD/{CG_PROD}
        //[HttpGet("BuscarPorCG_PROD/{CG_PROD}")]
        //public async Task<ActionResult<List<Producto>>> BuscarPorCG_PROD(string CG_PROD)
        //{
        //    List<Producto> lDesProd = new List<Producto>();
        //    if (_context.Prod.Any())
        //    {
        //        lDesProd = await _context.Prod.Where(p => p.CG_PROD == CG_PROD).ToListAsync();
        //    }
        //    if (lDesProd == null)
        //    {
        //        return NotFound();
        //    }
        //    return lDesProd;
        //}

        // GET: api/Prevision/BuscarPorDES_PROD/{DES_PROD}
        //[HttpGet("BuscarPorDES_PROD/{DES_PROD}")]
        //public async Task<ActionResult<List<Producto>>> BuscarPorDES_PROD(string DES_PROD)
        //{
        //    List<Producto> lDesProd = new List<Producto>();
        //    if (_context.Prod.Any())
        //    {
        //        lDesProd = await _context.Prod.Where(p => p.DES_PROD == DES_PROD).ToListAsync();
        //    }
        //    if (lDesProd == null)
        //    {
        //        return NotFound();
        //    }
        //    return lDesProd;
        //}

        // GET: api/Prevision/AgregarProductoPrevision/{CG_PROD}/{DES_PROD}
        [HttpGet("BuscarProductoPrevision/{CG_PROD}/{DES_PROD}/{Busqueda}")]
        public async Task<ActionResult<List<Producto>>> BuscarProductoPrevision(string CG_PROD, string DES_PROD, int Busqueda)
        {
            List<Producto> lContiene = new();
            if (DES_PROD == "Vacio")
            {
                lContiene = (List<Producto>)await _productoRepository.Obtener(p => p.CG_PROD.Contains(CG_PROD));
                if (lContiene == null)
                {
                    return NotFound();
                }

                lContiene = lContiene.Take(Busqueda).ToList();
                
                
            }
            else if (CG_PROD == "Vacio")
            {
                lContiene = (List<Producto>)await _productoRepository.Obtener(p => p.DES_PROD.Contains(DES_PROD));

                if (lContiene == null)
                {
                    return NotFound();
                }
                lContiene = lContiene.Take(Busqueda).ToList();
                
            }
            //else if (CG_PROD != "Vacio" && DES_PROD != "Vacio")
            //{
            //    if (_context.Prod.Any())
            //    {
            //        lContiene = await _context.Prod.Where(p => p.CG_PROD.Contains(CG_PROD) && p.DES_PROD.Contains(DES_PROD)).Take(Busqueda).ToListAsync();
            //    }
            //    if (lContiene == null)
            //    {
            //        return NotFound();
            //    }
            //}
            return lContiene;
        }

        // GET: api/Prevision/AgregarProductoPrevision/{CG_PROD}/{DES_PROD}
        [HttpPost("AgregarProductoPrevision")]
        public async Task<IActionResult> AgregarProductoPrevision(Producto parametros)
        {
            try
            {
                string xFecha = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
                if (!await _productoRepository.Existe(parametros.CG_PROD.Trim()))
                {
                    return NotFound();
                }

                try
                {
                    await _previsionRepository.AgregarBySP(parametros);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

                return Ok(await _previsionRepository.ObtenerTodos());
            }
            catch (DbUpdateConcurrencyException dbex)
            {
                return BadRequest(dbex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // GET: api/Prevision/AgregarProductoPrevision/{CG_PROD}
        [HttpGet("AgregarProductoPrevision/{CG_ART}")]
        public async Task<ActionResult<IEnumerable<PresAnual>>> AgregarProductoPrevision(string CG_ART)
        {
            try
            {
                const string xCantidad = "1";
                string xFecha = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");


                // Averigua unidad
                if (!await _productoRepository.Existe(CG_ART.Trim()))
                {
                    return NotFound();
                }

                try
                {
                    var prod = await _productoRepository.ObtenerPorId(CG_ART.Trim());
                    await _previsionRepository.AgregarBySP(prod);
                    var previsiones = await _previsionRepository.ObtenerTodos();
                    return Ok(previsiones.FirstOrDefault());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

                
            }
            catch (DbUpdateConcurrencyException dbex)
            {
                return BadRequest(dbex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Prevision/BorrarPrevision/{REGISTRO}
        [HttpGet("BorrarPrevision/{REGISTRO}")]
        public async Task<ActionResult<IEnumerable<PresAnual>>> BorrarPrevision(int REGISTRO)
        {
            await _previsionRepository.Remover(REGISTRO);
            return await _previsionRepository.ObtenerTodos();
        }

        // PUT: api/Prevision/PutPrev/{id}
        [HttpPut("PutPrev/{id}")]
        public async Task<IActionResult> PutPrev(int id, PresAnual prev)
        {
            if (id != prev.REGISTRO)
            {
                return BadRequest();
            }


            try
            {
                await _previsionRepository.Actualizar(prev);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _previsionRepository.Existe(id))
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

    }
}
