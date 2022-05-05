using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Data.Repository;
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
    }
}
