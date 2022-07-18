using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [ApiController()]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsuariosController(AppDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuarios>>> Get()
        {
            return await _context.Users
                .Select(u => new Usuarios() { Id = u.Id, Usuario = u.UserName }).ToListAsync();
        }


        [HttpGet("roles")]
        public async Task<ActionResult<List<Rol>>> GetRoles()
        {
            return await _context.Roles
                .Select(r => new Rol { Id = r.Id, Descripcion = r.Name }).ToListAsync();

        }

        [HttpPost("roles")]
        public async Task<ActionResult> PostRoles(Rol rol)
        {

            try
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(rol.Descripcion));
                return Ok(roleResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [HttpPost("asignarRol")]
        public async Task<ActionResult<Usuarios>> Post(RolUsuario rol)
        {
            try
            {
                var adminRoleExists = await roleManager.FindByIdAsync(rol.RolId);
                if (adminRoleExists == null)
                {
                    //_logger.LogInformation("Adding Admin role");
                    var roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Select the user, and then add the admin role to the user
                var user = await userManager.FindByIdAsync(rol.UserId);
                if (!await userManager.IsInRoleAsync(user, adminRoleExists.Name))
                {
                    //_logger.LogInformation("Adding sysadmin to Admin role");
                    var userResult = await userManager.AddToRoleAsync(user, adminRoleExists.Name);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
