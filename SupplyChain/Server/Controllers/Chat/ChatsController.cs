using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SupplyChain.Server.Hubs;
using SupplyChain.Shared;
using SupplyChain.Shared.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        public ChatsController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet("{contactId}")]
        public async Task<ActionResult<List<ChatMessage>>> GetConversationAsync(string contactId)
        {
            try
            {
                var userId = HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).Select(a => a.Value).FirstOrDefault();
                var messages = await _context.ChatMessages
                        .Where(h => (h.FromUserId == contactId && h.ToUserId == userId)
                            || (h.FromUserId == userId && h.ToUserId == contactId))
                        .OrderBy(a => a.CreatedDate)
                        //.Include(a=>a.Foto)
                        .Include(a => a.FromUser)
                        .Include(a => a.ToUser)
                        .Select(x => new ChatMessage
                        {
                            FromUserId = x.FromUserId,
                            Message = x.Message,
                            //Foto =x.Foto,
                            CreatedDate = x.CreatedDate,
                            Id = x.Id,
                            ToUserId = x.ToUserId,
                            //ToUser = x.ToUser,
                            NameToUser = x.ToUser.UserName,
                            //FromUser = x.FromUser,
                            NameFromUser = x.FromUser.UserName,
                            Visto = x.Visto
                        }).ToListAsync();

                foreach (var item in messages.Where(m => !m.Visto))
                {
                    item.Visto = true;
                    _context.Entry(item).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
                return messages;
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("NoView")]
        public async Task<IActionResult> GetAllConversationsNoViewAsync()
        {
            try
            {
                var userId = HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).Select(a => a.Value).FirstOrDefault();
                var messages = await _context.ChatMessages
                        .Where(h => !h.Visto && h.ToUserId == userId)
                        .OrderBy(a => a.CreatedDate)
                        .Include(a => a.FromUser)
                        .Include(a => a.ToUser)
                        .Select(x => new ChatMessage
                        {
                            FromUserId = x.FromUserId,
                            Message = x.Message,
                            //Foto = x.Foto,
                            CreatedDate = x.CreatedDate,
                            Id = x.Id,
                            ToUserId = x.ToUserId,
                            ToUser = x.ToUser,
                            FromUser = x.FromUser
                        }).ToListAsync();
                return Ok(messages);

            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<Usuario>>> GetUsersAsync()
        {
            try
            {
                var userId = User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).Select(a => a.Value).FirstOrDefault();
                var allUsers = await _context.Users.Where(user => user.Id != userId)
                    .Select(s => new Usuario() { Id = s.Id, Nombre = s.UserName, Email = s.Email, Foto = s.Foto }).ToListAsync();
                return allUsers;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserDetailsAsync(string userId)
        {
            var user = await _context.Users.Where(user => user.Id == userId)
                .Select(s=> new Usuario() { Id = s.Id, Nombre = s.UserName, Email = s.Email, Foto = s.Foto}).FirstOrDefaultAsync();
            return Ok(user);
        }

        //[HttpGet("users/{userId}/{foto}")]
        //public async Task<ActionResult<List<vUsuario>>> GetUserFoto(string userId, byte foto)
        //{
        //    var user = await _context.Users.Where(user => user.Id == userId)
        //        .Select(s => new vUsuario { Id= s.Id, FOTO = s.Foto});
        //}
        [HttpPost]
        public async Task<ActionResult<ChatMessage>> SaveMessageAsync(ChatMessage message)
        {
            try
            {
                var userId = User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).Select(a => a.Value).FirstOrDefault();
                message.FromUserId = userId;
                message.CreatedDate = DateTime.Now;
                message.ToUser = await _context.Users.Where(user => user.Id == message.ToUserId).FirstOrDefaultAsync();
                await _context.ChatMessages.AddAsync(message);
                await _context.SaveChangesAsync();
                return message;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }

}
