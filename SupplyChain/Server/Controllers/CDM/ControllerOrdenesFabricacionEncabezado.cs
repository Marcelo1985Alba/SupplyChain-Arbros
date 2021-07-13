using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SupplyChain
{
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
                string xSQL = String.Format("SELECT C.CG_ORDF, " +
                "CASE WHEN C.FECHA_PREVISTA_FABRICACION IS NULL THEN '' ELSE C.FECHA_PREVISTA_FABRICACION END FECHA_PREVISTA_FABRICACION, " +
                "C.DIASFAB, C.HORASFAB, " +
                "C.CG_PROD,C.DES_PROD, C.CANT, convert(numeric(6, 2), (C.CANTFAB * 100 / C.CANT)) AS AVANCE, A.PEDIDO, " +
                "CASE WHEN A.ENTRPREV IS NULL THEN '' ELSE A.ENTRPREV END ENTRPREV, " +
                "A.CG_CLI, rtrim(A.DES_CLI) AS DES_CLI, A.CG_ART, A.DES_ART, A.CANTPED, " +
                "(SELECT descrip from Diccion where archivo='pedcli' and campo='campocom1') AS TITULO_CAMPOCOM1, " +
                "(SELECT descrip from Diccion where archivo='pedcli' and campo='campocom2') AS TITULO_CAMPOCOM2, " +
                "(SELECT descrip from Diccion where archivo='pedcli' and campo='campocom3') AS TITULO_CAMPOCOM3, " +
                "(SELECT descrip from Diccion where archivo='pedcli' and campo='campocom4') AS TITULO_CAMPOCOM4, " +
                "(SELECT descrip from Diccion where archivo='pedcli' and campo='campocom5') AS TITULO_CAMPOCOM5, " +
                "(SELECT descrip from Diccion where archivo='pedcli' and campo='campocom6') AS TITULO_CAMPOCOM6, " +
                "A.CAMPOCOM1, A.CAMPOCOM2, A.CAMPOCOM3, A.CAMPOCOM4, A.CAMPOCOM5, A.CAMPOCOM6, " +
                "A.OBSERITEM, rtrim(A.DIRENT) AS DIRENT, A.CG_TRANS, rtrim(B.DES_TRANS) AS DES_TRANS, rtrim(B.DIRTRANS) AS DIRTRANS," +
                "P.CG_ORDEN " +
                "FROM PROGRAMA C " +
                "LEFT JOIN PROD P ON C.CG_PROD = P.CG_PROD " +
                "LEFT JOIN PEDCLI A ON A.PEDIDO = C.PEDIDO " +
                "LEFT JOIN TRANSP B ON A.CG_TRANS = B.CG_TRANS " +
                "WHERE C.CG_ORDF = {0}"
                , idOrden);

                var ordenFab = await _context.OrdenesFabricacionEncabezado.FromSqlRaw(xSQL).FirstOrDefaultAsync();

                return ordenFab;
            }
            catch(Exception ex)
            {
                //return BadRequest(ex.Message);
                return new ModeloOrdenFabricacionEncabezado();
            }
        }
    }
}
