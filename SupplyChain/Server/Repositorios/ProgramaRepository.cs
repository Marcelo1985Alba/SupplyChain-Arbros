using Microsoft.AspNetCore.Builder;
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
    public class ProgramaRepository : Repository<Programa, decimal>
    {
        Repositorios.PedidosRepository PedidosRepository;
        public ProgramaRepository(AppDbContext db, PedidosRepository pedidosRepository) : base(db)
        {
            PedidosRepository = pedidosRepository;
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

        public async Task<IEnumerable<Programa>>GetOrdenesAbiertas(int cg_ordfasoc, int cg_ordf)
        {
            string xSQL = $"select * from programa where CG_ORDFASOC = {cg_ordfasoc} and CG_ESTADOCARGA < 4  " +
                            $"and cg_ordf < {cg_ordf} order by CG_ORDF ASC";
            return await base.DbSet.FromSqlRaw(xSQL).ToListAsync();

        }

        //public async Task<IEnumerable<Programa>> PostEstadoFirme( int cg_estadocarg, DateTime fe_curso, int cg_estado, int cg_ordf, int cg_ordfasoc)
        //{
        //    string xSQL = $"UPDATE Programa SET CG_ESTADOCARGA = {cg_estadocarg}, " +
        //            $"Fe_curso = {fe_curso}, CG_ESTADO = {cg_estado} WHERE (Cg_ordf = {cg_ordf} OR Cg_ordfAsoc = {cg_ordfasoc})";
            
        //    await base.Database.ExecuteSqlRawAsync(xSQL);
            
        //    return await DbSet.Where(p=>p.CG_ORDF==cg_ordf || p.CG_ORDFASOC==cg_ordfasoc).ToListAsync();

        //}

        public async Task<IEnumerable<Programa>> ActualizaCantidadFabricado(int cg_ordf, int cantfab)
        {
            string xSQL = $"update programa set CANT={cantfab} from programa where CG_ESTADO=4 AND CG_ORDF={cg_ordf}";
            await base.Database.ExecuteSqlRawAsync(xSQL);
            
            return await DbSet.Where(p=>p.CG_ORDF==cg_ordf).ToListAsync();
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

                    //var res = await query.FirstOrDefaultAsync();

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

                    //carga de stock
                    i.StockReal = await Db.ResumenStock.Where(r =>
                            r.CG_ART.ToUpper() == i.CG_ART.ToUpper() && (r.CG_DEP == 4 || r.CG_DEP == 15))
                    .SumAsync(s=> s.STOCK);

                    i.Reserva = await PedidosRepository.ObtenerStockReservaByOF(cg_ordf, i.CG_ART);
                    i.ReservaTotal = await PedidosRepository.ObtenerStockReserva(i.CG_ART);
                });



                return itemAbastecimiento;
            }
            catch (Exception ex)
            {
                return new List<ItemAbastecimiento>();
            }

        }

        public async Task<IEnumerable<Programa>> GetOrdenesAbiertas(int cg_ordfasoc, int cg_ordf)
        {
            string xSQL = $"select * from programa where CG_ORDFASOC = {cg_ordfasoc} and CG_ESTADOCARGA < 4  " +
                            $"and cg_ordf < {cg_ordf} order by CG_ORDF ASC";
            return await base.DbSet.FromSqlRaw(xSQL).ToListAsync();

        }

        //public async Task<IEnumerable<Programa>> PutCantidadFab(int cg_ordfasoc, int cantf, int cantidad)
        //{
        //    string xSQL = $"update programa set cantfab={cantidad} where cg_ordfasoc={cg_ordfasoc} ";
        //    return await base.DbSet.FromSqlRaw(xSQL).ToListAsync();
        //}

        //public async Task<IEnumerable<Programa>> GetCantidad(int cg_ordfasoc, int cg_ordf)
        //{
        //    string xSQL = $"select CANT* from Programa where CG_ORDFASOC={cg_ordfasoc} and CG_ORDF={cg_ordf}";
        //    return await base.DbSet.FromSqlRaw(xSQL).ToListAsync();
        //}
    }
}
