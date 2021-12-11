using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class ProgramaRepository : Repository<Programa, int>
    {
        public ProgramaRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<bool> Existe(int id)
        {
            return await base.DbSet.AnyAsync(e => e.REGISTRO == id);
        }

        public async Task<IEnumerable<Programa>> GetProgramasPedidos()
        {
            string xSQL = string.Format("SELECT Programa.* FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) where(pedidos.FLAG = 0 AND Programa.CG_ESTADO = 3 " +
                "AND Pedidos.CG_ORDF != 0 AND(Pedidos.TIPOO = 1)) UNION SELECT Programa.* " +
                "FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) " +
                "where Pedcli.PEDIDO NOT IN(select PEDIDO from Pedidos where TIPOO = 1) " +
                "AND Programa.CG_ESTADO = 3  AND Pedcli.CANTPED > 0 AND Pedidos.TIPOO != 28");
            return await base.DbSet.FromSqlRaw(xSQL).ToListAsync();
        }

        public async Task<IEnumerable<ItemAbastecimiento>> GetAbastecimientoByOF(int cg_ordf)
        {
            //var dt = new DataTable();
            List<ItemAbastecimiento> itemAbastecimiento;
            const string usuario = "user";

            try
            {
                var of = new SqlParameter("of", cg_ordf);
                itemAbastecimiento = await base.Db.Set<ItemAbastecimiento>()
                    .FromSqlRaw($"Exec dbo.NET_PCP_TraerAbast  @Cg_Ordf={cg_ordf}")
                    .ToListAsync();

                //Cargar Depositos: ver como cargar en sp
                await itemAbastecimiento.ForEachAsync(async i =>
                {
                    var query = Db.vResumenStock.Where(r =>
                    r.CG_ART.ToUpper() == i.CG_ART.ToUpper()
                    && r.LOTE.ToUpper() == i.LOTE.ToUpper()
                    && r.DESPACHO.ToUpper() == i.DESPACHO.ToUpper()
                    && r.SERIE.ToUpper() == i.SERIE.ToUpper()
                    && r.STOCK > 0
                    ).AsQueryable();

                    var res = await query.FirstOrDefaultAsync();

                    if (i.CG_DEP > 0)
                        query = query.Where(r => r.CG_DEP == i.CG_DEP);

                    if (i.CG_DEP == 0)
                        i.CG_DEP = 0;
                    else
                    {
                        var rs = await query.FirstOrDefaultAsync();
                        i.ResumenStock = rs;
                        i.CG_DEP = rs.CG_DEP;
                    }

                });



                return itemAbastecimiento;
            }
            catch (Exception ex)
            {
                return new List<ItemAbastecimiento>();
            }

        }

        
    }
}
