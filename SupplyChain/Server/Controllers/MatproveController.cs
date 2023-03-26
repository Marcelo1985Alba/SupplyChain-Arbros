using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChain;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatproveController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatproveController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/Matprove/BuscarProve/{CG_MAT}
        [HttpGet("BuscarProve/{CG_MAT}")]
        public async Task<ActionResult<IEnumerable<Matprove_busquedaprove>>> BuscarProve(string CG_MAT)
        {
            /*
            string xSQL = string.Format("SELECT a.NROCLTE, a.CG_MAT, a.CG_MAT1, a.DES_MAT1, a.CANT, " +
                "(case when a.UNID <> '' and a.cg_den > 0 then a.unid else c.unid end) as unid, " +
                "(case when a.UNID <> '' and a.cg_den > 0 then a.cg_den else c.cg_Den end) as cg_den, " +
                "(case when a.UNID <> '' and a.cg_den > 0 then a.unid1 else c.unid1 end) as unid1, " +
                "a.CANTAUTOR, a.ENTREGA, a.PRECIO, a.PRECIO2, a.FE_PREC, a.MONEDA, a.DIASVIGE, " +
                "a.CONDPREC, a.CONDVEN, a.BON11, a.CANTMIN, a.CANTLOTE, a.USUARIO, a.ACTIVO, " +
                "a.FE_REG, a.REGISTRO, b.CG_PROVE, b.DESCRIPCION as DES_PROVE FROM matprove as a " +
                "inner join vProveItris as b on a.nroclte = b.cg_prove " +
                "inner join materia as c on a.CG_MAT = c.CG_MAT " +
                "where a.activo = 1 and a.nroclte > 0 and precio > 0 and a.cg_mat = '" + CG_MAT + "' ");
            return await _context.Matprove_busquedaprove.FromSqlRaw(xSQL).ToListAsync();
            */
            List<Matprove_busquedaprove> respuesta = await _context.Matprove_busquedaprove
                .FromSqlRaw($"EXEC P_List_LlenaPrepCompras_Blazor '" + CG_MAT +"'").ToListAsync();

            return respuesta;
        }

        [HttpPost("GetProveedores")]
        public async Task<ActionResult<IEnumerable<vProveedorItris>>> GetProve(List<Compra> sugerencias)
        {
            try
            {
                var idsMateriaPrima = sugerencias.Select(s=> $"'{s.CG_MAT.Trim()}'").ToArray();
                var ids = string.Join(",", idsMateriaPrima);

                string xSQL = string.Format("SELECT CG_PROVE, DESCRIPCION, CUIT, NOMBRE_CONTACTO, EMAIL_CONTACTO  " +
                    "FROM vProveItris " +
                    $"WHERE CG_PROVE in (select NROCLTE from Matprove where cg_mat in ({ids}) GROUP BY NROCLTE)");

                return await _context.vProveedoresItris.FromSqlRaw(xSQL)
                     .ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
