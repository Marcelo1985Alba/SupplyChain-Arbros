using Microsoft.AspNetCore.SignalR;
using SupplyChain.Shared;
using System.Threading.Tasks;

namespace SupplyChain.Server.Hubs
{
    public class SolicitudHub : Hub
    {
        public async Task SendMessage(vSolicitudes vSolicitud)
        {
            await Clients.All.SendAsync("ReceiveVSolicitud", vSolicitud);
        }
    }
}
