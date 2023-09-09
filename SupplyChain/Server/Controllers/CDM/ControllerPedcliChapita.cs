using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class PedcliChapitaController : ControllerBase
{
    private readonly AppDbContext _context;

    public PedcliChapitaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IEnumerable<PedCli> Get(string CG_ART)
    {
        try
        {
            var xSQL = string.Format("select LOTE from Pedcli where CG_ART = '{0}'", CG_ART);
            return _context.PedCli.FromSqlRaw(xSQL).ToList();
        }
        catch
        {
            return new List<PedCli>();
        }
    }
}