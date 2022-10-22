using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupplyChain.Server.Hubs;
using SupplyChain.Shared;
using SupplyChain.Shared.Login;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly IHubContext<OnlineUsersHub> _hubOnlineUsersContext;
        private readonly AppDbContext _context;

        public CuentasController(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IHubContext<OnlineUsersHub> hubContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this._hubOnlineUsersContext = hubContext;
            this._context = context;
        }

        [HttpGet("renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> RenovarToken()
        {
            var userInfo = new UserInfo()
            {
                UserName = HttpContext.User.Identity.Name
            };

            var user = await userManager.FindByNameAsync(userInfo.UserName);
            var roles = await userManager.GetRolesAsync(user);
            return BuildToken(user, roles);
        }

        [HttpPost("resetearpassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ResetearPassword(UserInfo userInfo)
        {
            var user = await userManager.FindByNameAsync(userInfo.UserName);
            if (user != null)
            {
                string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, resetToken, userInfo.Password);
                if (!result.Succeeded)
                {
                    return BadRequest("La contraseña no pudo ser reseteada!");
                }
            }

            return Ok("La contreseña fue reseteada correctamente!");
        }
        [HttpPost("crearrol")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IdentityRole>> CrearRol(IdentityRole rol)
        {
            var result = await roleManager.CreateAsync(rol);
            if (!result.Succeeded)
            {
                return BadRequest("El Rol no pudo ser creado!");
            }

            return rol;
        }

        [HttpPost("asignarrol")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AsignarRol(RolUser rolUser)
        {

            var user = await userManager.FindByNameAsync(rolUser.UserName);
            var result = await userManager.AddToRoleAsync(user, rolUser.RolName);
            if (!result.Succeeded)
            {
                return BadRequest("El Rol no pudo ser asigndo!");
            }

            return Ok("El rol fue asignado correctamente!");
        }

        

        [HttpPost("crear")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> CrearUsuario([FromBody] UserInfo userInfo)
        {
            var user = new ApplicationUser { UserName = userInfo.UserName, Cg_Cli = userInfo.Cg_Cli, Email = userInfo.Email };
            var result = await userManager.CreateAsync(user, userInfo.Password);
            if (result.Succeeded)
            {

                if (userInfo.Roles.Count > 0)
                {
                    foreach (var rol in userInfo.Roles)
                    {
                        var resultRoles = await userManager.AddToRoleAsync(user, rol);

                        if (!resultRoles.Succeeded)
                        {
                            return BadRequest(resultRoles.Errors.Select(s => s.Description));
                        }
                    }
                    
                }


                return BuildToken(user, userInfo.Roles);
            }
            else
            {
                return BadRequest(result.Errors.Select(s=> s.Description));
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo)
        {
            var result = await signInManager.PasswordSignInAsync(userInfo.UserName, userInfo.Password,
                isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                //await _hubOnlineUsersContext.Clients.All.SendAsync("UpdateOnlineUsers", 1);
                var user = await userManager.FindByNameAsync(userInfo.UserName);
                var roles = await userManager.GetRolesAsync(user);

                if (roles.Any(r=> r== "Cliente") && user.Cg_Cli ==0)
                {
                    return BadRequest("Solicite autorizacion");
                }

                return BuildToken(user, roles);
            }
            else
            {
                return BadRequest("Usuario o Contraseña invalida");
            }

        }


        [HttpGet("logout")]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }


        private UserToken BuildToken(ApplicationUser userInfo, IList<string> roles)
        {
            List<Claim> claims = AgregaClaims(userInfo, roles);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:llave"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddHours(12);

            JwtSecurityToken token = new(issuer: null,
                audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }

        private static List<Claim> AgregaClaims(ApplicationUser userInfo, IList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.UserName),
                new Claim(ClaimTypes.Name, userInfo.UserName),
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            return claims;
        }
    }
}
