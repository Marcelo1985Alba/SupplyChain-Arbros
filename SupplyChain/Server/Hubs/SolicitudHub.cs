using Microsoft.AspNetCore.SignalR;
using SupplyChain.Shared;
using System.Threading.Tasks;

namespace SupplyChain.Server.Hubs
{
    public class SolicitudHub : Hub
    {
        public async Task SendMessage(vSolicitudes vSolicitud)
        {
            var userName = Context.GetHttpContext().User.Identity.Name;

            await Clients.All.SendAsync("ReceiveVSolicitud", vSolicitud);
        }

        
    }
}
