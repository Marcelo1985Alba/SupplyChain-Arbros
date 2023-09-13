using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelosGenericosStringStringController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModelosGenericosStringStringController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{SQLcommandString}")]
        public async Task<IEnumerable<ModeloGenericoStringString>> Get(string SQLcommandString)
        {
            try
            {
                List<ModeloGenericoStringString> xResultado = await _context.ModelosGenericosStringString.FromSqlRaw(SQLcommandString)
                    .ToListAsync();
                return xResultado;
            }
            catch (Exception ex)
            {
                return new List<ModeloGenericoStringString>();
            }
        }
    }
}
