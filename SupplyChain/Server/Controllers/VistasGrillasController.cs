using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VistasGrillasController : ControllerBase
{
    private readonly AppDbContext _context;

    public VistasGrillasController(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    [HttpGet("GetByName/{name}")]
    public async Task<ActionResult<List<VistasGrillas>>> GetByName(string name)
    {
        var userName = HttpContext.User.Identity.Name;
        return await _context.VistasGrillas.Where(v => v.AppName == name && v.Usuario == userName).ToListAsync();
    }

    [HttpGet("GetByNameSyncf/{name}")]
    public object GetByNameSyncf(string name)
    {
        var data = _context.VistasGrillas
            .Where(v => v.AppName == name).AsQueryable();
        var count = data.Count();
        var queryString = Request.Query;
        if (queryString.Keys.Contains("$inlinecount"))
        {
            StringValues Skip;
            StringValues Take;
            var skip = queryString.TryGetValue("$skip", out Skip) ? Convert.ToInt32(Skip[0]) : 0;
            var top = queryString.TryGetValue("$top", out Take) ? Convert.ToInt32(Take[0]) : data.Count();
            return new { Items = data.Skip(skip).Take(top), Count = count };
        }

        return data;
    }

    [HttpPost]
    public async Task<ActionResult<VistasGrillas>> Post(VistasGrillas vistasGrillas)
    {
        var userName = HttpContext.User.Identity.Name;
        vistasGrillas.Usuario = userName;
        if (vistasGrillas.Id > 0)
            _context.Entry(vistasGrillas).State = EntityState.Modified;
        else
            _context.VistasGrillas.Add(vistasGrillas);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(vistasGrillas);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var query = _context.VistasGrillas.Where(v => v.Id == id);
        if (!query.Any()) return NotFound();

        _context.Remove(await query.FirstOrDefaultAsync());
        await _context.SaveChangesAsync();

        return Ok();
    }
}