using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class OrdenesFabricacionMPController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdenesFabricacionMPController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{idOrden}")]
    public async Task<IEnumerable<ModeloOrdenFabricacionMP>> Get(int idOrden)
    {
        try
        {
            var xSQL = string.Format("SELECT REGISTRO, CG_ART, DES_ART, " +
                                     "convert(numeric(10, 4), (STOCK * - 1)) AS STOCK, LOTE, DESPACHO, SERIE " +
                                     "FROM Pedidos WHERE TIPOO IN (10, 11, 28) AND CG_ORDEN = 4 AND CG_ORDF = {0} " +
                                     "ORDER BY REGISTRO", idOrden);
            return await _context.OrdenesFabricacionMP.FromSqlRaw(xSQL).ToListAsync();
        }
        catch
        {
            return new List<ModeloOrdenFabricacionMP>();
        }
    }
}