using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SupplyChain.Server.Hubs;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ChatsController> _logger;
        private readonly IHubContext<ChatHub> hubChatContext;

        public ChatsController(ILogger<ChatsController> logger, IHubContext<ChatHub> hubChatContext)
        {
            _logger = logger;
            this.hubChatContext = hubChatContext;
        }

        [HttpGet("SendMessage")]
        public async Task<Chat> SendMessage([FromQuery] Chat chat)
        {
            await hubChatContext.Clients.Group(chat.RECEIVERCONNECTIONID).SendAsync("ReceiveMessage", chat.USER, chat.MESSAGE);
            return chat;
        }
    }
}
