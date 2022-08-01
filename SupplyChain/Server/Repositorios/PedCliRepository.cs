using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Logística;
using SupplyChain.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PedCliRepository : Repository<PedCli, int>
    {
        private readonly vDireccionesEntregaRepository direccionesEntregaRepository;

        public PedCliRepository(AppDbContext db, vDireccionesEntregaRepository direccionesEntregaRepository) : base(db)
        {
            this.direccionesEntregaRepository = direccionesEntregaRepository;
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

        public async Task<PedCliEncabezado> ObtenerPedidosEncabezado(int id)
        {
            var ped = await DbSet.FindAsync(id);
            var pedEncabezado = new PedCliEncabezado();
            if (ped != null)
            {
                pedEncabezado.FE_MOV = ped.FE_PED;
                pedEncabezado.CONDICION_PAGO = ped.DPP;
                pedEncabezado.DIRENT = ped.DIRENT;
                pedEncabezado.CG_CLI = ped.CG_CLI;
                pedEncabezado.DES_CLI = ped.DES_CLI;
                pedEncabezado.FE_MOV = ped.FE_PED;
                pedEncabezado.PEDIDO = ped.PEDIDO;
                pedEncabezado.NUMOCI = ped.NUMOCI;
                pedEncabezado.MONEDA = ped.MONEDA;
                pedEncabezado.BONIFIC = ped.BONIFIC;
                pedEncabezado.TC = (double)ped.VA_INDIC;

                await AgregarDireccionesEntrega(pedEncabezado);

                pedEncabezado.Items.Add(ped);
            }
            return pedEncabezado;
        }

        public async Task AgregarDireccionesEntrega(PedCliEncabezado pedCliEncabezado)
        {
            pedCliEncabezado.DireccionesEntregas = await direccionesEntregaRepository
                    .Obtener(d => d.ID_CLIENTE == pedCliEncabezado.CG_CLI.ToString()).ToListAsync();

            if (!string.IsNullOrEmpty(pedCliEncabezado.DIRENT) && 
                !pedCliEncabezado.DireccionesEntregas.Any(d=> d.DESCRIPCION.ToUpper() == pedCliEncabezado.DIRENT.ToUpper()))
            {
                var dirEntrega = new vDireccionesEntrega();
                dirEntrega.Id = 999999999;
                dirEntrega.ID_CLIENTE = pedCliEncabezado.CG_CLI.ToString();
                dirEntrega.DESCRIPCION = pedCliEncabezado.DIRENT;
                pedCliEncabezado.DireccionesEntregas.Add(dirEntrega);
            }
        }


        public async Task GuardarList(List<PedCli> list)
        {
            foreach (PedCli item in list)
            {
                if (item.ESTADO == Shared.Enum.EstadoItem.Agregado)
                {
                    DbSet.Add(item);
                }

                if (item.ESTADO == Shared.Enum.EstadoItem.Modificado)
                {
                    Db.Entry(item).State = EntityState.Modified;
                }

                if (item.ESTADO == Shared.Enum.EstadoItem.Eliminado)
                {
                    Db.Remove(item);
                }
            }

            await SaveChanges();
        }

    }
}
