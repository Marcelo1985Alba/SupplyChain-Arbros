﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChain;
using SupplyChain.Shared.Models;

namespace SupplyChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgramaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Programa> Get()
        {
            string xSQL = string.Format("SELECT Programa.* FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) where(pedidos.FLAG = 0 AND Programa.CG_ESTADO = 3 " +
                "AND Pedidos.CG_ORDF != 0 AND(Pedidos.TIPOO = 1)) UNION SELECT Programa.* " +
                "FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) " +
                "where Pedcli.PEDIDO NOT IN(select PEDIDO from Pedidos where TIPOO = 1) " +
                "AND Programa.CG_ESTADO = 3  AND Pedcli.CANTPED > 0 AND Pedidos.TIPOO != 28");
            return _context.Programa.FromSqlRaw(xSQL).ToList();
        }


        [HttpGet("GetPedidos")]
        public IEnumerable<Programa> Gets(string PEDIDO)
        {
            string xSQL = string.Format("SELECT * FROM Programa ");
            return _context.Programa.FromSqlRaw(xSQL).ToList<Programa>();
        }


        // GET: api/Programas/GetPlaneadas
        [HttpGet("GetPlaneadas")]
        public async Task<ActionResult<IEnumerable<Programa>>> GetPlaneadas()
        {
            return await _context.Programa.Where(p => p.Cg_Cia == 1 && p.CG_ESTADOCARGA == 1 && p.CG_ORDF == p.CG_ORDFASOC)
                .OrderBy(r => r.CG_ORDF).ToListAsync();

        }

        // GET: api/Programas/GetAbastecimientoByOF/73411
        [HttpGet("GetAbastecimientoByOF/{cg_ordf:int}")]
        public async Task<ActionResult<IEnumerable<ItemAbastecimiento>>> GetAbastecimientoByOF(int cg_ordf)
        {
            var dt = new DataTable();
            List<ItemAbastecimiento> itemAbastecimiento;
            var usuario = "user";


            //using (var command = _context.Database.GetDbConnection().CreateCommand())
            //{
            //    var sp = $"Execute NET_PCP_TraerAbast {cg_ordf}, '{usuario}'";
            //    command.CommandText = sp; //$"Execute NET_PCP_TraerAbast";
            //    //command.CommandType = CommandType.StoredProcedure;
            //    //command.Parameters.Add(new SqlParameter("@Cg_Ordf", cg_ordf));
            //    //command.Parameters.Add(new SqlParameter("@Usuario", usuario));
            //    var conn = command.Connection;

            //    if (conn.State != ConnectionState.Open) conn.Open();

            //    using (var result = await command.ExecuteReaderAsync())
            //    {
            //        //dt.Load(result);

            //        //var eee = await Helpers.ConvertDataTable<ItemAbastecimiento>(dt);


            //    }
            //}

            //El modelo esta seteado en dcontext
            try
            {
                var of = new SqlParameter("of", cg_ordf);
                itemAbastecimiento = await _context.Set<ItemAbastecimiento>()
                    .FromSqlRaw("Exec dbo.NET_PCP_TraerAbast  @Cg_Ordf=@of", of)
                    .ToListAsync();

                return itemAbastecimiento;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // GET: api/Programas/GetProgramaByOF/cg_ordf
        [HttpGet("GetProgramaByOF/{cg_ordf}")]
        public async Task<ActionResult<IEnumerable<Programa>>> GetCompraByOF(decimal cg_ordf)
        {
            return await _context.Programa.Where(p => p.Cg_Cia == 1
                    && p.CG_ORDF == cg_ordf).ToListAsync();

        }

        // GET: api/Programas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Programa>> GetPrograma(decimal id)
        {
            var programa = await _context.Programa.FindAsync(id);

            if (programa == null)
            {
                return NotFound();
            }

            return programa;
        }

        [HttpGet("EnviarCsvDataCore")]
        public async Task<ActionResult> EnviarCsv()
        {
            string xSQL = string.Format("EXEC INTERFACE_DATACORE");
            await _context.Database.ExecuteSqlRawAsync(xSQL);
            return Ok();
        }
        // PUT: api/Programas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrograma(decimal id, Programa programa)
        {
            if (id != programa.REGISTRO)
            {
                return BadRequest();
            }

            _context.Entry(programa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgramaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Programas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Programa>> PostPrograma(Programa programa)
        {
            _context.Programa.Add(programa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrograma", new { id = programa.REGISTRO }, programa);
        }

        // DELETE: api/Programas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Programa>> DeletePrograma(decimal id)
        {
            var programa = await _context.Programa.FindAsync(id);
            if (programa == null)
            {
                return NotFound();
            }

            _context.Programa.Remove(programa);
            await _context.SaveChangesAsync();

            return programa;
        }

        private bool ProgramaExists(decimal id)
        {
            return _context.Programa.Any(e => e.REGISTRO == id);
        }


    }
}