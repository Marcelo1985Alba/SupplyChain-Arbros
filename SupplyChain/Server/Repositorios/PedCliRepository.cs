using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;
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


        public async Task<IEnumerable<PedCli>> ObtenerPedCliPedidos()
        {
            string xSQL = string.Format("SELECT Pedcli.*, CAST( (CASE WHEN Pedidos.FLAG = 0 THEN 0 ELSE 1 END) AS BIT) AS FLAG " +
                "FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) where(pedidos.FLAG = 0 AND Programa.CG_ESTADO = 3 " +
                "AND Pedidos.CG_ORDF != 0 AND(Pedidos.TIPOO = 1)) " +
                "UNION " +
                "SELECT Pedcli.*, CAST( (CASE WHEN Pedidos.FLAG = 0 THEN 0 ELSE 1 END) AS BIT) AS FLAG " +
                "FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) " +
                "where Pedcli.PEDIDO NOT IN(select PEDIDO from Pedidos where TIPOO = 1) " +
                "AND Programa.CG_ESTADO = 3  AND Pedcli.CANTPED > 0 AND Pedidos.TIPOO != 28");
            return await base.DbSet.FromSqlRaw(xSQL).ToListAsync();
        }

        public async Task<IEnumerable<ModeloPedidosPendientes>> ObtenerPedidosPedientes()
        {
            string xSQL = string.Format("EXEC NET_PCP_PEDIDOS");
            return await base.Db.ModeloPedidosPendientes.FromSqlRaw(xSQL).ToListAsync();
        }

    }
}
