using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SupplyChain.Shared;

namespace SupplyChain.Server.Hubs;

public class SolicitudHub : Hub
{
    public async Task SendMessage(vSolicitudes vSolicitud)
    {
        var userName = Context.GetHttpContext().User.Identity.Name;

        await Clients.All.SendAsync("ReceiveVSolicitud", vSolicitud);
    }
}