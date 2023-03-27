using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulosUsuarioController : ControllerBase
    {
        private readonly ModulosUsuarioRepository _modulosUsuarioRepository;
        private readonly ModuloRepository _moduloRepository;

        public ModulosUsuarioController(ModulosUsuarioRepository modulosUsuarioRepository, ModuloRepository moduloRepository)
        {
            this._modulosUsuarioRepository = modulosUsuarioRepository;
            this._moduloRepository = moduloRepository;
        }

        [HttpGet("GetModulosFromUserId/{userId}")]
        public async Task<ActionResult<List<Modulo>>> GetModulosFromUserId(string userId)
        {
            try
            {
                List<Modulo> modulos = new();
                var modulosUsuario = await _modulosUsuarioRepository.ObtenerTodosQueryable()
                    .Where(m => m.UserId == userId).ToListAsync();

                if (modulosUsuario is not null && modulosUsuario.Count > 0)
                {
                    modulos = await _moduloRepository.ObtenerTodosQueryable().Where(m =>
                        modulosUsuario.Select(s => s.ModuloId).Contains(m.Id)).ToListAsync();

                    modulos ??= new();
                }


                return Ok(modulos);
            }
            catch (Exception ex )
            {
                return BadRequest(ex);
            }
        }

        // POST: api/ModulosUsuario/PostList
        [HttpPost("PostList")]
        public async Task<ActionResult<Modulo>> PostList([FromBody] List<ModulosUsuario> modulosUsuarios)
        {

            try
            {
                await _modulosUsuarioRepository.GuardarLista(modulosUsuarios);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var modulos = _moduloRepository.ObtenerTodosQueryable()
                .Where(m=> modulosUsuarios.Select(s=> s.ModuloId).Contains(m.Id))
                .ToList();
            return Ok(modulos);
        }

    }
}
