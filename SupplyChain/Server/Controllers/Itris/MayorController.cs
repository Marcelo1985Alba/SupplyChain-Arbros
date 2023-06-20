using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Data;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Itris;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.RichTextEditor;

namespace SupplyChain.Server.Controllers.Itris
{
    [Route("api/[controller]")]
    [ApiController]
    public class MayorController : ControllerBase
    {
        private readonly ItrisDbContext _context;

        public MayorController(ItrisDbContext context)
        {
            this._context = context;
        }

        // GET: api/Compras
        [HttpGet]
        public IActionResult GetCompras()
        {
            try
            {
                var mayor = _context.vMayorItris
                    .Where(f => !f.CONCEPTO.ToUpper().StartsWith("ASIENTO") && !f.CONCEPTO.ToUpper().StartsWith("CMV") 
                                && 
                                (f.ID_1.ToString().StartsWith("4") || f.ID_1.ToString().StartsWith("5")) || f.ID_1.ToString().StartsWith("114"))
                    .ToList();


                var result = new
                {
                    Items = mayor,
                    Count = mayor.Count
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
