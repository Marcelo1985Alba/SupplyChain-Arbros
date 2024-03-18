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
        public ProcunController(ProcunRepository procunRepository,ProductoRepository productoRepository)
        {
            this._procunRepository = procunRepository;
           
            _productoRepository = productoRepository;
        }

        //GET: api/Procun
        [HttpGet("id")]
        public async Task<ActionResult<Procun>> GetProcunById(decimal id)
        {
            try
            {   //var listaprocun= await _procunRepository.ObtenerTodos();
                return await _procunRepository.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("GetvProcuns")]
        public async Task<ActionResult<IEnumerable<vProcun>>> GetvProcuns()
        {
            try
            {
                var listaprocun = await _procunRepository.ObtenerProcun();
                return Ok(listaprocun);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{Cg_Prod}")]
        public async Task<IEnumerable<vProcun>> GetvProcun(string Cg_Prod)
        {
            try
            {
                var listaprocun = await _procunRepository.ObtenerProcun();
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
        public async Task<ActionResult<vProcun>> PostProcun(Procun Proc)
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
                Proc.FRECU = 0;
                await _procunRepository.Agregar(Proc);
                return CreatedAtAction("GetProcunById", new { id = Proc.Id }, Proc);
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
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Procun/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Procun>> DeleteProcun(decimal id)
        //{
        //    var Proc = await _procunRepository.ObtenerPorId(id);
        //    if (Proc == null)
        //    {
        //        return NotFound();
        //    }

        //    await _procunRepository.Remover(id);

        //    return Proc;
        //}

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
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<Procun>>DeleteProcun(decimal id)
        {
            try
            {
                var procun = await _procunRepository.Obtener(p => p.Id == id).FirstOrDefaultAsync();

                if(procun == null)
                {
                    return NotFound();
                }

                await _procunRepository.Remover(id);
                return procun;
            }
            catch(Exception ex)
            {
                return BadRequest("Error al eliminar el proceso" + ex.Message);
            }
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

        [HttpPut("{id}")]
        public async Task<ActionResult<Procun>> ActualizarPro(decimal id, Procun procun)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;

                procun.USUARIO = username;
                procun.AUTORIZA= username;
                procun.CG_CATEOP=" ";
                procun.CG_AREA = 0;
                procun.CG_LINEA = 0;
                procun.DESCRIP=" ";
                procun.OBSERV=" ";
                procun.DESPROC = "";
                procun.FRECU = 0;
                procun.CG_CALI1 = 0;
                procun.CG_CALI2 = 0;
                procun.CG_CALI3 = 0;
                procun.CG_CALI4 = 0;
                procun.CG_CALI5 = 0;
                procun.CG_CALI6 = 0;
                procun.CG_CALI7 = 0;
                procun.CG_OPER = 0;
                procun.TOLE1= 0;
                procun.TOLE2 = 0;
                procun.VALOR1 = 0;
                procun.VALOR2 = 0;
                procun.VALOR3 = 0;
                procun.VALOR4 = 0;
                procun.TOLE3 = 0;
                procun.TOLE4 = 0;
                procun.COSTO = 0;
                procun.COSTOCOMB = 0;
                procun.COSTOENERG = 0;
                procun.PLANTEL = 0;
                procun.CG_CATEOP= " ";
                procun.COSTAC= 0;
                procun.OCUPACION= 0;
                procun.COEFI= 0;
                procun.ESTANDAR= false;
                procun.RELEVAN= 0;
                procun.REVISION= 0;
                procun.TAREAPROC = 0;

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

        
    }
}