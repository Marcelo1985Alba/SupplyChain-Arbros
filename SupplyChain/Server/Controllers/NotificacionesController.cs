using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionesController : ControllerBase
{
    private readonly ILogger<NotificacionSubscripcion> _logger;
    private readonly NotificacionRepository notificacionRepository;

    public NotificacionesController(ILogger<NotificacionSubscripcion> logger,
        NotificacionRepository notificacionRepository)
    {
        _logger = logger;
        this.notificacionRepository = notificacionRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<NotificacionSubscripcion>> Get()
    {
        return await notificacionRepository.ObtenerTodos();
    }

    [HttpPut]
    public async Task<NotificacionSubscripcion> Subscribe(NotificacionSubscripcion notificacionSubscripcion)
    {
        var userId = GetUserId();
        var oldsSubscribtion = notificacionRepository.Obtener(n => n.UserId == userId).ToArray();
        await notificacionRepository.RemoveRange(oldsSubscribtion);

        await notificacionRepository.Agregar(notificacionSubscripcion);
        return notificacionSubscripcion;
    }

    private string GetUserId()
    {
        return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}