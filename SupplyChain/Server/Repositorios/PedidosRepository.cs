using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PedidosRepository : Repository<Pedidos, int?>
    {
        private readonly CompraRepository _comprasRepository;
        public PedidosRepository(AppDbContext db) : base(db)
        {
            _comprasRepository = new CompraRepository(db);
        }


        public async Task<int> MaxNumeroVale(int cg_cia_usuario)
        {
            int numero = 1;

            if (await DbSet.AnyAsync())
                numero += await Obtener(p => p.CG_CIA == cg_cia_usuario).MaxAsync(p => p.VALE);


            return numero;
        }

        public async Task<decimal> ObtenerRecepSumByOcMp(int oCompra, string cg_mat)
        {
            return await DbSet.Where(p => p.TIPOO == 5 && p.OCOMPRA == oCompra && p.CG_ART == cg_mat)
                    .SumAsync(p => p.STOCK) ?? 0;
        }

        public async Task<List<Pedidos>> ObtenerByNumeroVale(int vale, int cg_cia_usuario)
        {
            

            var list = await Obtener(p => p.VALE == vale && p.CG_CIA == cg_cia_usuario)
                       .ToListAsync();
            //SI ES RECEPCION HAYQ EU CALCULAR PENDIENTE DE OC Y OBTENER EL PEDIDO
            if (list.Count > 0 && list[0].TIPOO == 5)
            {
                await list.ForEachAsync(async s =>
                {
                    var solicitado = await _comprasRepository.Obtener(c => c.NUMERO == s.OCOMPRA && c.CG_MAT == s.CG_ART)
                    .SumAsync(c => c.SOLICITADO);

                    var recibido = await Obtener(p => p.TIPOO == 5 && p.OCOMPRA == s.OCOMPRA && p.CG_ART == s.CG_ART)
                        .SumAsync(p => p.STOCK);

                    s.PENDIENTEOC = solicitado - recibido;

                    s.Proveedor = await Db.Proveedores.FirstOrDefaultAsync(p=> p.Id == s.CG_PROVE);
                });


            }

            if (list.Count > 0 && list[0].TIPOO == 10)
            {
                await list.ForEachAsync(async i =>
                {
                    i.ResumenStock = await Db.vResumenStock.Where(r =>
                         r.CG_ART.ToUpper() == i.CG_ART.ToUpper()
                         && r.LOTE.ToUpper() == i.LOTE.ToUpper()
                         && r.DESPACHO.ToUpper() == i.DESPACHO.ToUpper()
                         && r.SERIE.ToUpper() == i.SERIE.ToUpper()
                         && r.CG_DEP == i.CG_DEP).FirstOrDefaultAsync();

                    i.PENDIENTEOC = i.ResumenStock.STOCK - Math.Abs((decimal)i.STOCK);
                });


            }
            return list;

        }

        public async Task<PedidoEncabezado> ObtenerPedidosEncabezado(int id)
        {
            var ped = await DbSet.FindAsync(id);
            var pedEncabezado = new PedidoEncabezado();
            if (ped != null)
            {
                pedEncabezado.FE_MOV = ped.FE_MOV;
                pedEncabezado.CG_CONDICION_PAGO = ped.CG_CONDICION_PAGO;
                pedEncabezado.DIRENT = ped.DIRENT;
                pedEncabezado.CG_CLI = (int)ped.CG_CLI;
                pedEncabezado.DES_CLI = ped.DES_CLI;
                pedEncabezado.PEDIDO = (int)ped.PEDIDO;
                pedEncabezado.CG_TRANS = ped.CG_TRANS;
                pedEncabezado.CG_COND_ENTREGA = ped.CG_COND_ENTREGA;
                pedEncabezado.MONEDA = ped.MONEDA;
                pedEncabezado.BONIFIC = (decimal)ped.BONIFIC;
                pedEncabezado.VA_INDIC = ped.VA_INDIC;
                pedEncabezado.REMITO = ped.REMITO;

                pedEncabezado.Items.Add(ped);
            }
            return pedEncabezado;
        }

        public async Task<PedidoEncabezado> GetListaByPedidos(List<int> listaPedidos)
        {
             var pedidos = await Db.vPedidoAltas.Where(p => listaPedidos.Contains(p.PEDIDO)).ToListAsync();
                var pedEncabezado = new PedidoEncabezado();
                if (pedidos != null && pedidos.Count > 0)
                {
                    var cabacera = pedidos.First();
                    pedEncabezado = new PedidoEncabezado()
                    {
                        BONIFIC = (decimal)cabacera.BONIFIC,
                        MONEDA = cabacera.MONEDA,
                        CG_CLI = cabacera.CG_CLI,
                        CG_CONDICION_PAGO = cabacera.DPP,
                        DES_CLI = cabacera.DES_CLI,
                        CG_COND_ENTREGA = cabacera.CG_COND_ENTREGA,
                        CG_TRANS = cabacera.CG_TRANS,
                        DIRENT = cabacera.DIRENT,
                        VA_INDIC = cabacera.VA_INDIC,
                        VOUCHER = 0,
                    };
                    var id = 0;
                    foreach (var item in pedidos)
                    {
                        id--;
                        var pedido = new Pedidos
                        {
                            Id = id,
                            FE_MOV = DateTime.Now,
                            CG_CONDICION_PAGO = pedEncabezado.CG_CONDICION_PAGO,
                            DIRENT = pedEncabezado.DIRENT,
                            CG_CLI = pedEncabezado.CG_CLI,
                            DES_CLI = pedEncabezado.DES_CLI,
                            CG_TRANS = pedEncabezado.CG_TRANS,
                            CG_COND_ENTREGA = pedEncabezado.CG_COND_ENTREGA,
                            MONEDA = pedEncabezado.MONEDA,
                            BONIFIC = pedEncabezado.BONIFIC,
                            VA_INDIC = pedEncabezado.VA_INDIC,

                            CG_ART = item.CG_ART,
                            DES_ART = item.DES_ART,
                            STOCK = item.STOCK,
                            PEDIDO = item.PEDIDO,
                            CG_CIA = 1,
                            CG_DEP = 1,
                            CG_ORDF = item.CG_ORDF,
                            DESCUENTO = item.DESCUENTO,
                            IMPORTE1 = item.IMPORTE1,
                            IMPORTE2 = item.IMPORTE2,
                            IMPORTE3 = item.IMPORTE3,
                            IMPORTE4 = item.IMPORTE4,
                            IMPORTE6 = item.IMPORTE6,
                            DESPACHO = item.DESPACHO,
                            LOTE = item.LOTE,
                            SERIE = item.SERIE,

                        };


                        pedEncabezado.Items.Add(pedido);
                    }
                }


                return pedEncabezado;
            
        }

        internal async Task<List<Pedidos>> GetRemitos(TipoFiltro tipoFiltro = TipoFiltro.Todos , int cg_cia = 1)
        {
            if (tipoFiltro == TipoFiltro.Pendientes)
            {
                var query = $"SELECT * FROM Pedidos Where tipoo = 1 And Cg_Cia = {cg_cia} " +
                    "AND VOUCHER = 0 AND Factura = '' " +
                    $"AND REMITO NOT IN(SELECT REMITO FROM Pedidos WHERE tipoo = 1 And Cg_Cia = {cg_cia} " +
                    "AND VOUCHER <> 0 ) " +
                    "ORDER BY FE_MOV DESC";

                return await DbSet.FromSqlRaw(query).ToListAsync();
            }


            return await Obtener(p => p.TIPOO == 1 && p.VOUCHER == 0 && p.CG_CIA == cg_cia, 0,
                         s => s.OrderByDescending(p => p.FE_MOV), true)
                .ToListAsync();
        }
    }
}
