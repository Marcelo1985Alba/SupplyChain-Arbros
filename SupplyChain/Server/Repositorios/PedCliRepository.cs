using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PedCliRepository : Repository<PedCli, int>
    {
        public PedCliRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<bool> Existe(int id)
        {
            return await base.DbSet.AnyAsync(e => e.REGISTRO == id);
        }
        public async Task<IEnumerable<PedCli>> ObtenerPedCliPedidos()
        {
            string xSQL = string.Format("SELECT Pedcli.* FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) where(pedidos.FLAG = 0 AND Programa.CG_ESTADO = 3 " +
                "AND Pedidos.CG_ORDF != 0 AND(Pedidos.TIPOO = 1)) " +
                "UNION " +
                "SELECT Pedcli.* FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) " +
                "where Pedcli.PEDIDO NOT IN(select PEDIDO from Pedidos where TIPOO = 1) " +
                "AND Programa.CG_ESTADO = 3  AND Pedcli.CANTPED > 0 AND Pedidos.TIPOO != 28");
            return await base.DbSet.FromSqlRaw(xSQL).ToListAsync();
        }
    }
}
