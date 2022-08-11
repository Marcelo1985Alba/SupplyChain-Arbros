﻿using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Logística;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PedCliRepository : Repository<PedCli, int>
    {
        private readonly vDireccionesEntregaRepository direccionesEntregaRepository;
        private readonly GeneraRepository generaRepository;

        public PedCliRepository(AppDbContext db, vDireccionesEntregaRepository direccionesEntregaRepository, 
            GeneraRepository generaRepository) : base(db)
        {
            this.direccionesEntregaRepository = direccionesEntregaRepository;
            this.generaRepository = generaRepository;
        }

        public async Task<IEnumerable<PedCli>> ByFilter(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            if (tipoFiltro == TipoFiltro.Pendientes)
            {
                var query = "Select * from Pedcli WHERE " +
                    "PEDIDO NOT IN (SELECT PEDIDO FROM PEDIDOS WHERE remito <> '' AND TIPOO = 1 ) " +
                    "AND CG_ESTADO NOT IN ('C') AND CANTPED > 0 AND IMPORTE1 > 0";
                return await base.DbSet.FromSqlRaw(query).ToListAsync();
            }
            else if (tipoFiltro == TipoFiltro.NoPendientes)
            {
                var query = "Select * from Pedcli WHERE " +
                    "PEDIDO IN (SELECT PEDIDO FROM PEDIDOS WHERE REMITO <> '' AND TIPOO = 1 )";
                return await base.DbSet.Where(p => p.CG_ESTADO == "C").ToListAsync();
            }


            return await base.ObtenerTodos();

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
                pedEncabezado.CG_TRANS = ped.CG_TRANS;
                pedEncabezado.CG_COND_ENTREGA = ped.CG_COND_ENTREGA;
                pedEncabezado.NUMOCI = ped.NUMOCI;
                pedEncabezado.MONEDA = ped.MONEDA;
                pedEncabezado.BONIFIC = ped.BONIFIC;
                pedEncabezado.ORCO = ped.ORCO;
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
            const string NUMOCI = "NUMOCI";
            const string PEDIDO = "PEDIDO";
            const string REGPED = "REGPED";

            try
            {
                await generaRepository.Reserva(NUMOCI);
                var genera = await generaRepository.Obtener(g => g.Id == NUMOCI).FirstOrDefaultAsync();

                foreach (PedCli item in list)
                {

                    item.NUMOCI = Convert.ToInt32(genera.VALOR1);

                    if (item.ESTADO == Shared.Enum.EstadoItem.Agregado)
                    {
                        //PEDIDO
                        await generaRepository.Reserva(PEDIDO);
                        var generaPedido = await generaRepository.Obtener(g => g.Id == PEDIDO).FirstOrDefaultAsync();
                        //REGISTRO
                        await generaRepository.Reserva(REGPED);
                        var generaRegPedido = await generaRepository.Obtener(g => g.Id == REGPED).FirstOrDefaultAsync();

                        item.PEDIDO = Convert.ToInt32(generaPedido.VALOR1);
                        item.Id = Convert.ToInt32(generaRegPedido.VALOR1);
                        Db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
                        Db.Add(item);


                        await generaRepository.Libera(PEDIDO);
                        await generaRepository.Libera(REGPED);
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

                await generaRepository.Libera(NUMOCI);
                await SaveChanges();

                //lo ejecuta despues de obtener el numero de pedido
                await AsignarServicio(list);
            }
            catch (Exception ex)
            {
                await generaRepository.Libera(NUMOCI);
                await generaRepository.Libera(PEDIDO);
                await generaRepository.Libera(REGPED);
            }
        }

        private async Task AsignarServicio(List<PedCli> list)
        {
            foreach (var item in list.Where(p => p.CG_ART.StartsWith("0012")))
            {

                if (item.CG_ART.StartsWith("0012") && item.PRESUPUESTOID > 0)
                {
                    var servicio = Db.Servicios.Where(s => s.PRESUPUESTO == item.PRESUPUESTOID).FirstOrDefault();
                    servicio.FECHA = DateTime.Now;
                    servicio.PEDIDO = item.PEDIDO;
                    Db.Entry(servicio).State = EntityState.Modified;
                    Db.Entry(servicio).Property(p=> p.PEDIDO).IsModified = true;
                    Db.Entry(servicio).Property(p=> p.FECHA).IsModified = true;
                    await Db.SaveChangesAsync();
                }
            }
        }
    }
}
