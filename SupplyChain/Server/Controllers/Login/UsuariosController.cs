using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Login;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosRepository _usuariosRepository;

        public UsuariosController(UsuariosRepository usuariosRepository)
        {
            _usuariosRepository = usuariosRepository;
        }

        [HttpGet("{usuario}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(string usuario)
        {
            try
            {
                var user = await _usuariosRepository.GetByUserName(usuario);
                return user == null ? NotFound() : user;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{usuario}/{contras}")]
        public async Task<ActionResult<Usuarios>> Get(string usuario, string contras)
        {
            try
            {
                return await _usuariosRepository.GetByUsernamePass(usuario, contras);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Usuarios>> Post([FromBody] Usuarios usuario)
        {
            try
            {
                var user = await _usuariosRepository.GetByUsernamePass(usuario.Usuario, usuario.Contras);
                return user == null ? NotFound() : user;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
