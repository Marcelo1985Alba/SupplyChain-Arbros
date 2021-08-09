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
using SupplyChain.Shared.Prod;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesFabricacionHojaRutaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenesFabricacionHojaRutaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{cgProd}/{cant}")]
        public async Task<IEnumerable<ModeloOrdenFabricacionHojaRuta>> Get(string cgProd, double cant)
        {
            try
            {
                string xSQL = string.Format("SELECT A.ORDEN, A.PROCESO, B.DESCRIP, " +
                                            "A.CG_CELDA, C.DES_CELDA, " +
                                            "(CASE WHEN A.PROPORC = 'N' THEN A.TIEMPO1 ELSE A.TIEMPO1 * {0} END) AS TIEMPO_TOTAL, " +
                                            "A.PROPORC, A.TIEMPO1 FROM Procun A, Protab B, Celdas C " +
                                            "WHERE A.PROCESO = B.PROCESO AND A.CG_CELDA = C.CG_CELDA AND A.CG_PROD = '{1}' ORDER BY A.ORDEN"
                                            , cant.ToString().Replace(",", ".")
                                            , cgProd);
                return await _context.OrdenesFabricacionHojaRuta.FromSqlRaw(xSQL).ToListAsync();
            }
            catch
            {
                return new List<ModeloOrdenFabricacionHojaRuta>();
            }
        }

        [HttpGet("GetByFilter")]
        public async Task<IEnumerable<ModeloOrdenFabricacionHojaRuta>> GetByFilter([FromQuery] FilterHojaRuta filterHojaRuta)
        {
            try
            {
                string xSQL = string.Format("SELECT A.ORDEN, A.PROCESO, B.DESCRIP, " +
                                            "A.CG_CELDA, C.DES_CELDA, " +
                                            "(CASE WHEN A.PROPORC = 'N' THEN A.TIEMPO1 ELSE A.TIEMPO1 * {0} END) AS TIEMPO_TOTAL, " +
                                            "A.PROPORC, A.TIEMPO1 FROM Procun A, Protab B, Celdas C " +
                                            "WHERE A.PROCESO = B.PROCESO AND A.CG_CELDA = C.CG_CELDA AND A.CG_PROD = '{1}' ORDER BY A.ORDEN"
                                            , filterHojaRuta.Cantidad.ToString().Replace(",", ".")
                                            , filterHojaRuta.CodigoProd);
                return await _context.OrdenesFabricacionHojaRuta.FromSqlRaw(xSQL).ToListAsync();
            }
            catch
            {
                return new List<ModeloOrdenFabricacionHojaRuta>();
            }
        }
    }
}
