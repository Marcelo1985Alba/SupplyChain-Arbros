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
        public async Task<IActionResult> GetConversationAsync(string contactId)
        {
            var userId = User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).Select(a => a.Value).FirstOrDefault();
            var messages = await _context.ChatMessages
                    .Where(h => (h.FromUserId == contactId && h.ToUserId == userId) || (h.FromUserId == userId && h.ToUserId == contactId))
                    .OrderBy(a => a.CreatedDate)
                    .Include(a => a.FromUser)
                    .Include(a => a.ToUser)
                    .Select(x => new ChatMessage
                    {
                        FromUserId = x.FromUserId,
                        Message = x.Message,
                        CreatedDate = x.CreatedDate,
                        Id = x.Id,
                        ToUserId = x.ToUserId,
                        ToUser = x.ToUser,
                        FromUser = x.FromUser
                    }).ToListAsync();
            return Ok(messages);
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<Usuario>>> GetUsersAsync()
        {
            var userId = User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).Select(a => a.Value).FirstOrDefault();
            var allUsers = await _context.Users.Where(user => user.Id != userId)
                .Select(s=> new Usuario() { Id = s.Id, Nombre = s.UserName, Email = s.Email }).ToListAsync();
            return allUsers;
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserDetailsAsync(string userId)
        {
            var user = await _context.Users.Where(user => user.Id == userId)
                .Select(s=> new Usuario() { Id = s.Id, Nombre = s.UserName, Email = s.Email }).FirstOrDefaultAsync();
            return Ok(user);
        }
        [HttpPost]
        public async Task<ActionResult<ChatMessage>> SaveMessageAsync(ChatMessage message)
        {
            var userId = User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).Select(a => a.Value).FirstOrDefault();
            message.FromUserId = userId;
            message.CreatedDate = DateTime.Now;
            message.ToUser = await _context.Users.Where(user => user.Id == message.ToUserId).FirstOrDefaultAsync();
            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }
    }

}
