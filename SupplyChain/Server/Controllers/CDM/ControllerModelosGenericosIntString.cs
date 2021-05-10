using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelosGenericosIntStringController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModelosGenericosIntStringController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{SQLcommandString}")]
        public async Task<IEnumerable<ModeloGenericoIntString>> Get(string SQLcommandString)
        {
            try
            {
                List<ModeloGenericoIntString> xResultado = await _context.ModelosGenericosIntString.FromSqlRaw(SQLcommandString)
                    .ToListAsync();
                return xResultado;
            }
            catch
            {
                return new List<ModeloGenericoIntString>();
            }
        }
    }
}
