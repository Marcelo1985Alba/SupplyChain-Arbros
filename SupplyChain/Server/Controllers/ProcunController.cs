using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcunController : ControllerBase
    {
        private readonly ProcunRepository _procunRepository;
        private readonly ProductoRepository _productoRepository;

        public ProcunController(ProcunRepository procunRepository)
        {
            this._procunRepository = procunRepository;
        }

        // GET: api/Procun
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Procun>>> GetProcun()
        {
            try
            {   //var listaprocun= await _procunRepository.ObtenerTodos();
                return await _procunRepository.ObtenerTodos();
            }                                                               
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
        // GET: api/Procun/Cg_Prod
        [HttpGet("{Cg_Prod}")]
        public async Task<IEnumerable<Procun>> GetProcun(string Cg_Prod)
        {
            try
            {
                var listaprocun = await _procunRepository.ObtenerTodos();
                return listaprocun.Where(x => x.CG_PROD == Cg_Prod).ToList();
            }                                                               
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("Existe/{id}")]
        public async Task<ActionResult<bool>> ExisteProcun(decimal id)
        {
            try
            {
                return await _procunRepository.Existe(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

      

        // POST: api/Procun
        [HttpPost]
        public async Task<ActionResult<Procun>> PostProcun(Procun Proc)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;

                Proc.USUARIO=username;
                Proc.AUTORIZA=username;
                Proc.FECHA = DateTime.Today;
                Proc.DESCRIP = " ";
                Proc.OBSERV = " ";
                Proc.DESPROC = " ";
                Proc.TAREAPROC = 1;
                Proc.CG_CATEOP = " ";
                Proc.CG_FORM = 1;
                await _procunRepository.Agregar(Proc);
                return CreatedAtAction("GetProcun", new { id = Proc.Id }, Proc);
            }
            catch (DbUpdateException exx)
            {
                if (!await _procunRepository.Existe(Proc.Id))
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

        // DELETE: api/Procun/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Procun>> DeleteProcun(decimal id)
        {
            var Proc = await _procunRepository.ObtenerPorId(id);
            if (Proc == null)
            {
                return NotFound();
            }

            await _procunRepository.Remover(id);

            return Proc;
        }

        [HttpPost("PostList")]
        public async Task<ActionResult<Procun>> PostList(List<Procun> Procs)
        {
            try
            {
                foreach (var item in Procs)
                {
                    await _procunRepository.Remover(item.Id);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok();
        }
        
        //POST: api/Procun/PostList2
        [HttpPost("PostList2")]
        public async Task<ActionResult<List<Procun>>> PostList2([FromBody] List<Procun> lista)
        {
            try
            {
                foreach (Procun proc in lista)
                {
                    await _procunRepository.Agregar(proc);
                }
                await _procunRepository.AgregarList(lista);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPut("ActualizaCelda/{id}/{cg_celda}")]
        //public async Task<ActionResult<Procun>> ActualizaCelda(decimal id, string cg_celda)
        //{
        //    try
        //    {
        //        var lista = await _procunRepository.ActualizaCelda(id, cg_celda);
        //        return Ok(lista);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }
        //}
        //[HttpPut("ActualizaProceso/{id}/{proceso}")]
        //public async Task<ActionResult<Procun>> ActualizaProceso(decimal id, string proceso)
        //{
        //    try
        //    {
        //        var lista = await _procunRepository.ActualizaProceso(id, proceso);
        //        return Ok(lista);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }
        //}


        [HttpGet("Search/{idProd}/{Des_Prod}")]
        public async Task<ActionResult<IEnumerable<Procun>>> Search(string idProd, string Des_Prod)
        {
            try
            {
                return Ok(await _procunRepository.Search(idProd, Des_Prod));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarPro(Procun procun)
        {
            try
            {
                await _procunRepository.Actualizar(procun);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _procunRepository.Existe(procun.Id))
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
            return Ok(procun);
        }


        //PUT: api/Procun/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutProcun(decimal id, Procun Proc)
        //{
        //    if (id != Proc.Id)
        //    {
        //        return BadRequest();
        //    }

        //    try
        //    {
        //        await _procunRepository.Actualizar(Proc);
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!await _procunRepository.Existe(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }
        //    return Ok(Proc);
        //}
    }
}