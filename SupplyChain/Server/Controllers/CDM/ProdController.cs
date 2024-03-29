﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.Prod;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdController : ControllerBase
    {
        private readonly ProductoRepository _productoRepository;

        public ProdController(ProductoRepository productoRepository)
        {
            this._productoRepository = productoRepository;
        }

        // GET: api/Prod
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProd()
        {
            try
            {
                return await _productoRepository.ObtenerTodos();
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Prod
        [HttpGet("ByTipo/{conMP}/{conSE}/{conPT}")]
        public async Task<ActionResult<IEnumerable<Producto>>> Get(bool conMP, bool conSE, bool conPT)
        {
            List<int> cg_0rdenValues = new List<int>();
            var query = _productoRepository.ObtenerTodosQueryable();
            try
            {
                if (conMP && conSE && conPT)
                {
                    query = query;
                }
                else
                {
                    if (conMP)
                    {
                        cg_0rdenValues.Add(4);
                    }

                    if (conSE)
                    {
                        cg_0rdenValues.Add(3);
                    }

                    if (conPT)
                    {
                        cg_0rdenValues.Add(1);
                    }
                    query = query.Where(p => cg_0rdenValues.Contains(p.CG_ORDEN));
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Productos/BuscarProducto/{CG_PROD}/{DES_PROD}
        [HttpGet("BuscarProducto/{CG_PROD}/{DES_PROD}/{Busqueda}")]
        public async Task<ActionResult<List<Producto>>> BuscarProducto(string CG_PROD, string DES_PROD, int Busqueda)
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
            else if (string.IsNullOrEmpty(CG_PROD) || CG_PROD == "Vacio")
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

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> GetProd(string id)
        {
            try
            {
                return await _productoRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("GetProdAndReparaciones")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProdAndReparaciones()
        {
            try
            {
                return await _productoRepository
                    .Obtener(p=> p.CG_ORDEN == 1 || p.CG_ORDEN == 13).ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        // GET: api/Prod/5
        [HttpGet("ExisteProducto/{id}")]
        public async Task<ActionResult<bool>> ExisteProducto(string id)
        {
            var existe = await _productoRepository.Existe(id);

            return existe;
        }


        // GET: api/Prod/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> Get(string id)
        {
            var Prod = await _productoRepository.ObtenerPorId(id);

            return Prod ?? (ActionResult<Producto>)NotFound();
        }

        // GET: api/Prod/5
        [HttpGet("GetByFilter")]
        public async Task<ActionResult<Producto>> GetByFilter([FromQuery]FilterProd filter)
        {
            try
            {
                var Prod = await _productoRepository.ObtenerPorId(filter.Codigo);

                return Prod ?? (ActionResult<Producto>)NotFound();
            }
            catch (Exception ex) 
            {
                return NotFound();
            }
        }


        // PUT: api/Prod/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<IActionResult> PutProd( Producto Prod)
        {
            //if (id != Prod.Id)
            //{
            //    return BadRequest();
            //}

            try
            {
                await _productoRepository.Actualizar(Prod);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _productoRepository.Existe(Prod.Id))
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
            return Ok(Prod);
        }

        // POST: api/Prod
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProd(Producto Prod)
        {
            try
            {
                if (Prod.CG_ORDEN == 1)
                {
                    Prod.EXIGESERIE = true;
                    Prod.EXIGEOA = true;
                }
                else if (Prod.CG_ORDEN == 3)
                {
                    Prod.EXIGELOTE = true;
                }
                else if (Prod.CG_ORDEN == 4)
                {
                    Prod.EXIGELOTE = true;
                }


                await _productoRepository.Agregar(Prod);
                return CreatedAtAction("GetProd", new { id = Prod.Id }, Prod);
            }
            catch (DbUpdateException exx) 
            {
                if (!await _productoRepository.Existe(Prod.Id))
                {
                    return Conflict();
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
        }

        [HttpPost("PostList")]
        public async Task<ActionResult<Producto>> PostList(List<Producto> productos)
        {
            try
            {
                foreach (var item in productos)
                {
                    await _productoRepository.Remover(item.Id.Trim());
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok();
        }


        // DELETE: api/Prod/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Producto>> DeleteProd(string id)
        {
            var Prod = await _productoRepository.ObtenerPorId(id);
            if (Prod == null)
            {
                return NotFound();
            }

            await _productoRepository.Remover(id);

            return Prod;
        }


        // GET: api/Prod/BuscarPorCG_PROD/{CG_PROD}
        [HttpGet("BuscarPorCG_PROD/{CG_PROD}")]
        public async Task<ActionResult<List<Producto>>> BuscarPorCG_PROD(string CG_PROD)
        {
            List<Producto> lDesProd = new List<Producto>();
            if (await _productoRepository.Existe(CG_PROD))
            {
                lDesProd = await _productoRepository.Obtener(p => p.Id == CG_PROD).ToListAsync();
            }
            return lDesProd == null ? NotFound() : lDesProd;
        }

        // GET: api/Prod/BuscarPorDES_PROD/{DES_PROD}
        [HttpGet("BuscarPorDES_PROD/{DES_PROD}")]
        public async Task<ActionResult<List<Producto>>> BuscarPorDES_PROD(string DES_PROD)
        {
            List<Producto> lDesProd = new List<Producto>();
            lDesProd = await _productoRepository.Obtener(p => p.DES_PROD == DES_PROD).ToListAsync();
            if (lDesProd == null)
            {
                return NotFound();
            }
            return lDesProd;
        }


        // GET: api/Prod/BuscarPorCG_PROD_PREP/{CG_PROD}
        [HttpGet("BuscarPorCG_PROD_PREP/{CG_PROD}")]
        public async Task<ActionResult<List<Producto>>> BuscarPorCG_PROD_PREP(string CG_PROD)
        {
            List<Producto> lDesProd = new List<Producto>();
            if (await _productoRepository.Existe(CG_PROD))
            {
                lDesProd = await _productoRepository.Obtener(p => p.Id == CG_PROD && p.CG_ORDEN != 1 && p.CG_ORDEN != 3).ToListAsync();
            }
            return lDesProd == null ? NotFound() : lDesProd;
        }

        // GET: api/Prod/BuscarPorDES_PROD_PREP/{DES_PROD}
        [HttpGet("BuscarPorDES_PROD_PREP/{DES_PROD}")]
        public async Task<ActionResult<List<Producto>>> BuscarPorDES_PROD_PREP(string DES_PROD)
        {
            List<Producto> lDesProd = new List<Producto>();
            lDesProd = await _productoRepository.Obtener(p => p.DES_PROD == DES_PROD && p.CG_ORDEN != 1 && p.CG_ORDEN != 3).ToListAsync();
            if (lDesProd == null)
            {
                return NotFound();
            }
            return lDesProd;
        }


        // GET: api/Productos/BuscarProducto_PREP/{CG_PROD}/{DES_PROD}
        [HttpGet("BuscarProducto_PREP/{CG_PROD}/{DES_PROD}/{Busqueda}")]
        public async Task<ActionResult<List<Producto>>> BuscarProducto_PREP(string CG_PROD, string DES_PROD, int Busqueda)
        {
            List<Producto> lContiene = new();
            if ((string.IsNullOrEmpty(CG_PROD) && string.IsNullOrEmpty(DES_PROD)) || (CG_PROD == "Vacio" && DES_PROD == "Vacio"))
            {
                /*
                lContiene = (await _productoRepository.ObtenerTodos())
                    .Take(Busqueda).ToList();
                */
                lContiene = await _productoRepository.Obtener(p => p.CG_ORDEN != 1 && p.CG_ORDEN != 3,Busqueda).ToListAsync();

            }
            else if (string.IsNullOrEmpty(DES_PROD) || DES_PROD == "Vacio")
            {
                lContiene = await _productoRepository.Obtener(p => p.Id.Contains(CG_PROD) && p.CG_ORDEN != 1 && p.CG_ORDEN != 3, Busqueda)
                    .ToListAsync();
                if (lContiene == null)
                {
                    return NotFound();
                }
            }
            else if (string.IsNullOrEmpty(CG_PROD) || CG_PROD == "Vacio")
            {
                lContiene = await _productoRepository.Obtener(p => p.DES_PROD.Contains(DES_PROD) && p.CG_ORDEN != 1 && p.CG_ORDEN != 3, Busqueda)
                    .ToListAsync();

                if (lContiene == null)
                {
                    return NotFound();
                }

            }

            else if (CG_PROD != "Vacio" && DES_PROD != "Vacio")
            {
                lContiene = await _productoRepository.Obtener(p => p.Id.Contains(CG_PROD)
                    && p.DES_PROD.Contains(DES_PROD) && p.CG_ORDEN != 1 && p.CG_ORDEN != 3, Busqueda).ToListAsync();

                if (lContiene == null)
                {
                    return NotFound();
                }
            }
            return lContiene;


        }

        [HttpGet("Search/{idProd}/{Des_producto}")]
        public async Task<ActionResult<List<Producto>>> Search(string idProd, string Des_producto)
        {
            try
            {
                return Ok(await _productoRepository.Search(idProd, Des_producto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
        

        // GET: api/Productos/BuscarProducto_PREP/{CG_PROD}/{DES_PROD}
        [HttpGet("Buscar")]
        public async Task<ActionResult<List<Producto>>> Buscar(string CG_PROD, string DES_PROD, int Busqueda)
        {
            List<Producto> lContiene = new();
            if ((string.IsNullOrEmpty(CG_PROD) && string.IsNullOrEmpty(DES_PROD)) || (CG_PROD == "VACIO" && DES_PROD == "VACIO"))
            {
                /*
                lContiene = (await _productoRepository.ObtenerTodos())
                    .Take(Busqueda).ToList();
                */
                lContiene = await _productoRepository.Obtener(p => p.CG_ORDEN != 1 && p.CG_ORDEN != 3, Busqueda).ToListAsync();

            }
            else if (string.IsNullOrEmpty(DES_PROD) || DES_PROD == "VACIO")
            {
                lContiene = await _productoRepository.Obtener(p => p.Id.Contains(CG_PROD) && p.CG_ORDEN != 1 && p.CG_ORDEN != 3, Busqueda)
                    .ToListAsync();
                if (lContiene == null)
                {
                    return NotFound();
                }
            }
            else if (string.IsNullOrEmpty(CG_PROD) || CG_PROD == "VACIO")
            {
                lContiene = await _productoRepository.Obtener(p => p.DES_PROD.Contains(DES_PROD) && p.CG_ORDEN != 1 && p.CG_ORDEN != 3, Busqueda)
                    .ToListAsync();

                if (lContiene == null)
                {
                    return NotFound();
                }

            }

            else if (CG_PROD != "VACIO" && DES_PROD != "VACIO")
            {
                lContiene = await _productoRepository.Obtener(p => p.Id.Contains(CG_PROD)
                    && p.DES_PROD.Contains(DES_PROD) && p.CG_ORDEN != 1 && p.CG_ORDEN != 3, Busqueda).ToListAsync();

                if (lContiene == null)
                {
                    return NotFound();
                }
            }
            return lContiene;
        }
        
        // GET: api/Productos/GetCG_DENSEG/{CG_PROD}
        [HttpGet("GetCG_DENSEG/{CG_PROD}")]
        public async Task<decimal> GetCG_DENSEG(string CG_PROD)
        {
            decimal? CG_DENSEG = 0;
            if (await _productoRepository.Existe(CG_PROD))
            {
                var resultCgDenseg = _productoRepository.Obtener(p => p.Id == CG_PROD).FirstOrDefaultAsync().Result.CG_DENSEG;
                if (resultCgDenseg != null)
                    CG_DENSEG = resultCgDenseg;
            }
            return (decimal)(CG_DENSEG);
        }

    }
}