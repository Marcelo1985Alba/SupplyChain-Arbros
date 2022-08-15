using Microsoft.AspNetCore.SignalR;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageAsync(ChatMessage message, string userName)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, userName);
        }
        public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
        {
            await Clients.All.SendAsync("ReceiveChatNotification", message, receiverUserId, senderUserId);
        }
    }
}
