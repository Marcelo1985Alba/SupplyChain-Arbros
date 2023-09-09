﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SupplyChain.Shared;

namespace SupplyChain.Server.Hubs;

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