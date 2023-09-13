using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Hubs
{
    public class OnlineUsersHub : Hub
    {
        private static readonly ConcurrentDictionary<string, bool> _onlineUsers = new ConcurrentDictionary<string, bool>();

        public override async Task OnConnectedAsync()
        {
            var userGuid = Context.GetHttpContext().User.Identity.Name;

            _onlineUsers.TryAdd(userGuid, true);

            await UpdateOnlineUsers(0);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userGuid = Context.GetHttpContext().User.Identity.Name;


            //try to remove key from dictionary
            if (!_onlineUsers.TryRemove(userGuid, out _))
                //if not possible to remove key from dictionary, then try to mark key as not existing in cache
                _onlineUsers.TryUpdate(userGuid, false, true);

            await UpdateOnlineUsers(0);

            await base.OnDisconnectedAsync(exception);
        }

        public Task UpdateOnlineUsers(int count)
        {
            count = GetOnlineUsersCount();
            return Clients.All.SendAsync("UpdateOnlineUsers", count);
        }

        public static int GetOnlineUsersCount()
        {
            return _onlineUsers.Count(p => p.Value);
        }
    }
}
