using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupplyChain.Client.Pages.Ventas._3_Presupuestos;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SupplyChain.Server.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class PresupuestosController : ControllerBase
    {
        private string CadenaConexionSQL = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
        private readonly PresupuestoAnteriorRepository _presupuestoAnteriorRepository;
        private readonly PresupuestoRepository _presupuestoRepository;
        private readonly GeneraRepository _generaRepository;

        public PresupuestosController(PresupuestoAnteriorRepository presupuestoAnteriorRepository, 
            PresupuestoRepository presupuestoRepository, 
            GeneraRepository generaRepository)
        {
            _presupuestoAnteriorRepository = presupuestoAnteriorRepository;
            _presupuestoRepository = presupuestoRepository;
            this._generaRepository = generaRepository;
        }
        // GET: api/<PresupuestosController>
        [HttpGet]
        public async Task<List<Presupuesto>> GetPresupuesto()
        {
            return await _presupuestoRepository.ObtenerTodosQueryable().Include(p=> p.Items).ToListAsync();
        }

        // GET: api/<PresupuestosController>
        [HttpGet("TienePedido/{id}")]
        public async Task<bool> TienePedido(int id)
        {
            try
            {
                return await _presupuestoRepository.TienePedido(id);

            }
             catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        [HttpGet("GetPresupuestoVista/{tipoFiltro}")]
        public async Task<List<vPresupuestos>> GetPresupuestoVista(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            return await _presupuestoRepository.GetForView(tipoFiltro);
        }

     
        // GET api/<PresupuestosController>/5
        [HttpGet("{id}")]
        public async Task<Presupuesto> GetPresupuesto(int id)
        {
            var presup = await _presupuestoRepository.Obtener(p => p.Id == id).Include(p => p.Items)
                //.ThenInclude(i=> i.Solicitud)
                .FirstOrDefaultAsync();

            
            await _presupuestoRepository.AgregarDatosFaltantes(presup);


            return presup;
        }

        // POST api/<PresupuestosController>
        [HttpPost]
        public async Task<ActionResult<Presupuesto>> Post(Presupuesto presupuesto)
        {
            return await AgregarNuevoPresupuesto(presupuesto);
        }

        private async Task<ActionResult<Presupuesto>> AgregarNuevoPresupuesto(Presupuesto presupuesto)
        {
            foreach (var item in presupuesto.Items)
            {
                item.Id = 0;
            }
            //await _presupuestoRepository.BeginTransaction();
            try
            {
                var userName = HttpContext.User.Identity.Name;
                presupuesto.USUARIO = userName;
                await _presupuestoRepository.Agregar(presupuesto);
                await _presupuestoRepository.ActualizarCalculoConPresupuestoByIdCalculo(presupuesto.Id);
                //await _presupuestoRepository.CommitTransaction();
                
            }
            catch (Exception ex)
            {
                var details = ex.Message;
                details += ex.InnerException?.Message;

                return BadRequest(details);
            }

            return CreatedAtAction("GetPresupuesto", new { id = presupuesto.Id }, presupuesto);
        }
        
        [HttpPost("PostFromSolicitud")]
        public async Task<ActionResult<Presupuesto>> PostFromSolicitud(Presupuesto presupuesto)
        {
            try
            {
                await _presupuestoRepository.AgregarDatosFaltantes(presupuesto);

                return await AgregarNuevoPresupuesto(presupuesto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/<PresupuestosController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Presupuesto>> Put(int id, Presupuesto presupuesto)
        {
            try
            {
                await _presupuestoRepository.Actualizar(presupuesto);
                await _presupuestoRepository.AgregarEliminarActualizarDetalles(presupuesto.Items);
                await _presupuestoRepository.ActualizarCalculoConPresupuestoByIdCalculo(presupuesto.Id);
                return Ok(presupuesto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("ActualizarColor/{id}/{color}")]
        public async Task<ActionResult<Presupuesto>>ActualizarColor( int id, string color)
        {
            try
            {
                var lista = await _presupuestoRepository.ActualizarColor(id, color);
                return Ok(lista);
            } 
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        
        [HttpGet("EnviarMotivos/{id}/{motivo}")]
        public async Task<ActionResult<Presupuesto>>EnviarMotivos(int id, string motivo)
        {
            try
            {
                var lista = await _presupuestoRepository.EnviarMotivos(id, motivo);
                return Ok(lista);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("EnviarComentario/{id}/{comentario}")]
        public async Task<ActionResult<IEnumerable<Presupuesto>>> EnviarComentario(int id, string comentario)
        {
            try
            {
                var lista = await _presupuestoRepository.EnviarComentario(id, comentario);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("EnviarAviso/{id}/{aviso}")]
        public async Task<ActionResult<IEnumerable<Presupuesto>>> EnviarAviso(int id, string aviso)
        {
            try
            {
                var lista = await _presupuestoRepository.EnviarAviso(id, aviso);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        // DELETE api/<PresupuestosController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Presupuesto>> DeleteCompra(int id)
        {
            try
            {
                var presup = await _presupuestoRepository.Obtener(p => p.Id == id).Include(p => p.Items)
                                .FirstOrDefaultAsync();
                if (presup == null)
                {
                    return NotFound();
                }

                if (await _presupuestoRepository.TienePedido(id))
                {
                    return Conflict("El presupuesto tiene pedido asociado.");
                }


                await _presupuestoRepository.Remover(id);

                await _presupuestoRepository.DesvincularSolicitud(presup);

                return presup;
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar Presupuesto " + ex.Message);
            }
        }

       

    }
}
