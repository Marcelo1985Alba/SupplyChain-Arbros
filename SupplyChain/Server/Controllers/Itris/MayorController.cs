using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.Server.Data;

namespace SupplyChain.Server.Controllers.Itris;

[Route("api/[controller]")]
[ApiController]
public class MayorController : ControllerBase
{
    private readonly ItrisDbContext _context;

    public MayorController(ItrisDbContext context)
    {
        _context = context;
    }

    // GET: api/Compras
    [HttpGet]
    public IActionResult GetCompras()
    {
        try
        {
            var mayor = _context.vMayorItris
                .Where(f => (!f.CONCEPTO.ToUpper().StartsWith("ASIENTO") && !f.CONCEPTO.ToUpper().StartsWith("CMV")
                                                                         &&
                                                                         (f.ID_1.ToString().StartsWith("4") ||
                                                                          f.ID_1.ToString().StartsWith("5"))) ||
                            f.ID_1.ToString().StartsWith("114"))
                .ToList();


            var result = new
            {
                Items = mayor, mayor.Count
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}