using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModulosUsuarioController : ControllerBase
{
    private readonly ModuloRepository _moduloRepository;
    private readonly ModulosUsuarioRepository _modulosUsuarioRepository;

    public ModulosUsuarioController(ModulosUsuarioRepository modulosUsuarioRepository,
        ModuloRepository moduloRepository)
    {
        _modulosUsuarioRepository = modulosUsuarioRepository;
        _moduloRepository = moduloRepository;
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

                modulos ??= new List<Modulo>();
            }


            return Ok(modulos);
        }
        catch (Exception ex)
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
            .Where(m => modulosUsuarios.Select(s => s.ModuloId).Contains(m.Id))
            .ToList();
        return Ok(modulos);
    }
}