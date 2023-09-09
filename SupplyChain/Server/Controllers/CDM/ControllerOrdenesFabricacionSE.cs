using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class OrdenesFabricacionSEController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdenesFabricacionSEController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{idOrden}")]
    public async Task<IEnumerable<ModeloOrdenFabricacionSE>> Get(int idOrden)
    {
        try
        {
            var xSQL = string.Format("SELECT PE.REGISTRO, " +
                                     "PE.CG_ART, PE.DES_ART, PE.DESPACHO, " +
                                     "(PE.STOCK * - 1) AS STOCK, PE.LOTE, PE.VALE, PE.UBICACION, PR.CG_LINEA " +
                                     "FROM Pedidos PE, Prod PR WHERE PE.CG_ART = PR.CG_PROD AND PE.TIPOO IN (10, 11, 28) " +
                                     "AND PE.CG_ORDEN = 3 AND PE.CG_ORDF = {0} " +
                                     "ORDER BY PE.REGISTRO",
                idOrden);
            return await _context.OrdenesFabricacionSE.FromSqlRaw(xSQL).ToListAsync();
        }
        catch
        {
            return new List<ModeloOrdenFabricacionSE>();
        }
    }
}