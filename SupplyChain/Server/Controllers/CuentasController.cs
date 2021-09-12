using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly AppDbContext _context;

        public CuentasController(AppDbContext context, UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this._context = context;
        }


        [HttpPost("crear")]
        public async Task<ActionResult<UserToken>> CrearUsuario([FromBody] UserInfo userInfo)
        {
            var user = new IdentityUser { UserName = userInfo.UserName };
            var result = await userManager.CreateAsync(user, userInfo.Password);
            if (result.Succeeded)
            {
                return BuildToken(userInfo, new List<string>());
            }
            else
            {
                return BadRequest("Usuario o Contraseña invalida");
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo)
        {
            var result = await signInManager.PasswordSignInAsync(userInfo.UserName, userInfo.Password,
                isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(userInfo.UserName);
                var roles = await userManager.GetRolesAsync(user);
                return BuildToken(userInfo, roles);
            }
            else
            {
                return BadRequest("Usuario o Contraseña invalida");
            }

        }


        private UserToken BuildToken(UserInfo userInfo, IList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.UserName),
                new Claim(ClaimTypes.Name, userInfo.UserName),
                //new Claim(ClaimTypes.Role, "Admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:llave"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddHours(1);

            JwtSecurityToken token = new(issuer: null,
                audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            return new UserToken()
            {
                Token= new JwtSecurityTokenHandler().WriteToken(token),
                Expiration= expiration
            };
        }
    }
}
