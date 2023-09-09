using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SupplyChain;

[Route("api/[controller]")]
[ApiController]
public class OrdenesFabricacionEncabezadoController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdenesFabricacionEncabezadoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{idOrden}")]
    public async Task<ActionResult<ModeloOrdenFabricacionEncabezado>> Get(int idOrden)
    {
        try
        {
            var xSQL = string.Format(@"SELECT C.CG_ORDF, 
                CASE WHEN C.FECHA_PREVISTA_FABRICACION IS NULL THEN GETDATE() ELSE C.FECHA_PREVISTA_FABRICACION END FECHA_PREVISTA_FABRICACION,
                C.DIASFAB, C.HORASFAB, C.CG_PROD, C.DES_PROD, C.CANT, convert(numeric(6, 2), (C.CANTFAB * 100 / C.CANT)) AS AVANCE,
                CASE WHEN A.PEDIDO IS NULL THEN 0 ELSE A.PEDIDO END PEDIDO,
                CASE WHEN A.ENTRPREV IS NULL THEN GETDATE() ELSE A.ENTRPREV END ENTRPREV,
                CASE WHEN A.CG_CLI IS NULL THEN 0 ELSE A.CG_CLI END CG_CLI,
                CASE WHEN A.DES_CLI IS NULL THEN '' ELSE rtrim(A.DES_CLI) END AS DES_CLI,
                CASE WHEN A.CG_ART IS NULL THEN '' ELSE A.CG_ART END CG_ART,
                CASE WHEN A.DES_ART IS NULL THEN '' ELSE A.DES_ART END DES_ART,
                CASE WHEN A.CANTPED IS NULL THEN 0 ELSE A.CANTPED END CANTPED,
                (SELECT descrip from Diccion where archivo = 'pedcli' and campo = 'campocom1') AS TITULO_CAMPOCOM1,
                (SELECT descrip from Diccion where archivo = 'pedcli' and campo = 'campocom2') AS TITULO_CAMPOCOM2,
                (SELECT descrip from Diccion where archivo = 'pedcli' and campo = 'campocom3') AS TITULO_CAMPOCOM3,
                (SELECT descrip from Diccion where archivo = 'pedcli' and campo = 'campocom4') AS TITULO_CAMPOCOM4,
                (SELECT descrip from Diccion where archivo = 'pedcli' and campo = 'campocom5') AS TITULO_CAMPOCOM5,
                (SELECT descrip from Diccion where archivo = 'pedcli' and campo = 'campocom6') AS TITULO_CAMPOCOM6,
                CASE WHEN A.CAMPOCOM1 IS NULL THEN '' ELSE A.CAMPOCOM1 END CAMPOCOM1, 
                CASE WHEN A.CAMPOCOM2 IS NULL THEN '' ELSE A.CAMPOCOM2 END CAMPOCOM2, 
                CASE WHEN A.CAMPOCOM3 IS NULL THEN '' ELSE A.CAMPOCOM3 END CAMPOCOM3, 
                CASE WHEN A.CAMPOCOM4 IS NULL THEN '' ELSE A.CAMPOCOM4 END CAMPOCOM4, 
                CASE WHEN A.CAMPOCOM5 IS NULL THEN '' ELSE A.CAMPOCOM5 END CAMPOCOM5, 
                CASE WHEN A.CAMPOCOM6 IS NULL THEN '' ELSE A.CAMPOCOM6 END CAMPOCOM6,
                CASE WHEN A.OBSERITEM IS NULL THEN '' ELSE A.OBSERITEM END OBSERITEM, 
                CASE WHEN A.DIRENT IS NULL THEN '' ELSE rtrim(A.DIRENT) END AS DIRENT,
                CASE WHEN A.CG_TRANS IS NULL THEN 0 ELSE A.CG_TRANS END CG_TRANS,
                CASE WHEN B.DES_TRANS IS NULL THEN rtrim(B.DES_TRANS) END AS DES_TRANS, 
                CASE WHEN B.DIRTRANS IS NULL THEN rtrim(B.DIRTRANS) END AS DIRTRANS,
                P.CG_ORDEN  " +
                                     "FROM PROGRAMA C " +
                                     "LEFT JOIN PROD P ON C.CG_PROD = P.CG_PROD " +
                                     "LEFT JOIN PEDCLI A ON A.PEDIDO = C.PEDIDO " +
                                     "LEFT JOIN TRANSP B ON A.CG_TRANS = B.CG_TRANS " +
                                     "WHERE C.CG_ORDF = {0}"
                , idOrden);

            return await _context.OrdenesFabricacionEncabezado.FromSqlRaw(xSQL).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            //return BadRequest(ex.Message);
            return new ModeloOrdenFabricacionEncabezado();
        }
    }
}