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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

      

        

        // GET: api/Prevision/AgregarProductoPrevision/{CG_PROD}/{DES_PROD}
        [HttpGet("BuscarProductoPrevision/{CG_PROD}/{DES_PROD}/{Busqueda}")]
        public async Task<ActionResult<List<Producto>>> BuscarProductoPrevision(string CG_PROD, string DES_PROD, int Busqueda)
        {
            List<Producto> lContiene = new();
            if ((string.IsNullOrEmpty(CG_PROD) && string.IsNullOrEmpty(DES_PROD)) || (CG_PROD == "Vacio" && DES_PROD == "Vacio"))
            {
                lContiene = (await _productoRepository.ObtenerTodos())
                    .Take(Busqueda).ToList();
            }
            else if (string.IsNullOrEmpty(DES_PROD) || DES_PROD == "Vacio")
            {
                lContiene = await _productoRepository.Obtener(p => p.Id.Contains(CG_PROD), Busqueda)
                    .ToListAsync();
                if (lContiene == null)
                {
                    return NotFound();
                }

                
                
            }
            else if  (string.IsNullOrEmpty(CG_PROD) || CG_PROD == "Vacio")
            {
                lContiene = await _productoRepository.Obtener(p => p.DES_PROD.Contains(DES_PROD), Busqueda)
                    .ToListAsync();

                if (lContiene == null)
                {
                    return NotFound();
                }
                
            }
            

            else if (CG_PROD != "Vacio" && DES_PROD != "Vacio")
            {
                lContiene = await _productoRepository.Obtener(p => p.Id.Contains(CG_PROD)
                    && p.DES_PROD.Contains(DES_PROD), Busqueda).ToListAsync();

                if (lContiene == null)
                {
                    return NotFound();
                }
            }
            return lContiene;
        }

        // GET: api/Prevision/AgregarProductoPrevision/{CG_PROD}/{DES_PROD}
        [HttpPost("AgregarProductoPrevision")]
        public async Task<IActionResult> AgregarProductoPrevision(PresAnual parametros)
        {
            try
            {
                string xFecha = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd");
                if (!await _productoRepository.Existe(parametros.CG_ART.Trim()))
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
                //string xFecha = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");


                // Averigua unidad
                if (!await _productoRepository.Existe(CG_ART.Trim()))
                {
                    return NotFound();
                }

                try
                {
                    var prod = await _productoRepository.ObtenerPorId(CG_ART.Trim());
                    var presAnual = new PresAnual();
                    presAnual.CG_ART = prod.Id;
                    presAnual.DES_ART = prod.DES_PROD;
                    presAnual.ENTRPREV = DateTime.Now;
                    presAnual.CANTPED = 1;
                    await _previsionRepository.AgregarBySP(presAnual);
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
            if (id != prev.Id)
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
