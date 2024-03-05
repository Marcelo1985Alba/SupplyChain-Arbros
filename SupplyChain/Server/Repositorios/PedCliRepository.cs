using BoldReports.RDL.DOM;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.Pages.Servicio.Servicios;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Logística;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Data;
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
        private readonly PresupuestoRepository presupuestoRepository;

        public PedCliRepository(AppDbContext db, vDireccionesEntregaRepository direccionesEntregaRepository, 
            GeneraRepository generaRepository, PresupuestoRepository presupuestoRepository) : base(db)
        {
            this.direccionesEntregaRepository = direccionesEntregaRepository;
            this.generaRepository = generaRepository;
            this.presupuestoRepository = presupuestoRepository;
        }

        internal async Task<bool> TieneRemito(int pedido)
        {
            return await Db.Pedidos.AnyAsync(p=> (p.TIPOO == 1 && p.PEDIDO == pedido && p.REMITO != "") || 
                (p.TIPOO == 28 && p.PEDIDO == pedido) ||
                 (p.TIPOO == 28 && p.PEDIDO == pedido) );
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
            string xSQL = string.Format("SELECT Pedcli.DES_CLI,Pedcli.PRESUP,Pedcli.OBRA,Pedcli.PEDIDO,Pedcli.CG_ESTADPEDCLI,Pedcli.CG_ESTADO,Pedcli.FE_PED,Pedcli.ORDCAR,Pedcli.CG_CAM,Pedcli.CG_ART, " +
                "Pedcli.DESCUENTO, Pedcli.DES_ART,Pedcli.CANTPED,Pedcli.UNID,Pedcli.CANTEMP,Pedcli.CANTENT,Pedcli.IMPORTE1,Pedcli.IMPORTE2,Pedcli.IMPORTE3,pedcli.IMPORTE4,Pedcli.IMPORTE6,Pedcli.IVA, "+
                "Pedcli.ENTRPREV,pedcli.ENTRREAL,Pedcli.cg_cli,PEDIDOS.REMITO,Pedidos.Factura, Pedcli.CG_CATEG,Pedcli.ORCO, Pedcli.DPP,Pedcli.DFF,Pedcli.BONIFIC,Pedcli.CG_TRANS, " +
                "Pedcli.CG_ZONA, Pedcli.DIRENT,Pedcli.LOCALIDAD,Pedcli.CG_POST,Pedcli.CG_PROV,Pedcli.CG_VEN,Pedcli.LISTA,Pedcli.FACTCAMION,Pedcli.CONCARGO,pedcli.IMPRIMIBLE,Pedcli.GARANTIA,Pedcli.DES_OBRA, " +
                "Pedcli.SERVICIO,Pedcli.FE_VENC,Pedcli.CG_CLAS,Pedcli.CG_DEp,Pedcli.USUARIO,Pedcli.REGISTRO,Pedcli.FE_REG,Pedcli.MERMA,Pedcli.CG_CIA,Pedcli.NUMOCI,Pedcli.MONEDA,Pedcli.FE_INDIC, " +
                "Pedcli.VA_INDIC,Pedcli.RECAL,Pedcli.OBSERITEM,Pedcli.CG_ART1,Pedcli.CG_COT,Pedcli.COMBO,Pedcli.CG_EXPRESO,Pedcli.CG_EXPORT,Pedcli.CG_FORM,Pedcli.CAMPOCOM1,Pedcli.CAMPOCOM2,Pedcli.CAMPOCOM3, " +
                "Pedcli.CAMPOCOM4,Pedcli.CAMPOCOM5,Pedcli.CAMPOCOM6,Pedcli.CG_PROY,Pedcli.VIA,Pedcli.OBS1,Pedcli.OBS2,Pedcli.OBS3,Pedcli.OBS4,Pedcli.PROFORMA,Pedcli.MUESTRA,Pedcli.PREVORIG,Pedcli.DES_ART1," +
                "Pedcli.STOCKA,Pedcli.UNIDA,Pedcli.CANTENTA,Pedcli.CG_DEN,Pedcli.lote,Pedcli.despacho,Pedcli.serie,Pedcli.cg_pallet,Pedcli.campana,Pedcli.OCOMPRA,Pedcli.CANTPED_ORI,Pedcli.CG_POSTA, " +
                "Pedcli.DIRECC,Pedcli.ESTADO_IT,Pedcli.FE_AUT,Pedcli.USU_AUT,Pedcli.NOMRP,Pedcli.CG_FORM_VENTAS,Pedcli.CONDVEN,Pedcli.CG_COS,Pedcli.ESTADO_LOGISTICA,Pedcli.CAMPOCOM7,Pedcli.CAMPOCOM8, " +
                "Pedcli.CG_COND_ENTREGA, CAST( (CASE WHEN Pedidos.FLAG = 0 THEN 0 ELSE 1 END) AS BIT) AS FLAG FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
                "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) where(pedidos.FLAG = 0 AND Programa.CG_ESTADO = 3 AND Pedidos.CG_ORDF != 0 AND(Pedidos.TIPOO = 1)) UNION SELECT Pedcli.DES_CLI, " +
                "Pedcli.PRESUP,Pedcli.OBRA,Pedcli.PEDIDO,Pedcli.CG_ESTADPEDCLI,Pedcli.CG_ESTADO,Pedcli.FE_PED,Pedcli.ORDCAR,Pedcli.CG_CAM,Pedcli.CG_ART,Pedcli.DESCUENTO,Pedcli.DES_ART,Pedcli.CANTPED,Pedcli.UNID, " +
                "Pedcli.CANTEMP,Pedcli.CANTENT,Pedcli.IMPORTE1,Pedcli.IMPORTE2,Pedcli.IMPORTE3,pedcli.IMPORTE4,Pedcli.IMPORTE6,Pedcli.IVA,Pedcli.ENTRPREV,pedcli.ENTRREAL,Pedcli.cg_cli, " +
                "PEDIDOS.REMITO,Pedidos.Factura,Pedcli.CG_CATEG,Pedcli.ORCO, Pedcli.DPP,Pedcli.DFF,Pedcli.BONIFIC,Pedcli.CG_TRANS,Pedcli.CG_ZONA, Pedcli.DIRENT,Pedcli.LOCALIDAD,Pedcli.CG_POST,Pedcli.CG_PROV, " +
                "Pedcli.CG_VEN,Pedcli.LISTA,Pedcli.FACTCAMION,Pedcli.CONCARGO,pedcli.IMPRIMIBLE,Pedcli.GARANTIA,Pedcli.DES_OBRA,Pedcli.SERVICIO,Pedcli.FE_VENC,Pedcli.CG_CLAS,Pedcli.CG_DEp,Pedcli.USUARIO, " +
                "Pedcli.REGISTRO,Pedcli.FE_REG,Pedcli.MERMA,Pedcli.CG_CIA,Pedcli.NUMOCI,Pedcli.MONEDA,Pedcli.FE_INDIC,Pedcli.VA_INDIC,Pedcli.RECAL,Pedcli.OBSERITEM,Pedcli.CG_ART1,Pedcli.CG_COT, " +
                "Pedcli.COMBO,Pedcli.CG_EXPRESO,Pedcli.CG_EXPORT,Pedcli.CG_FORM,Pedcli.CAMPOCOM1,Pedcli.CAMPOCOM2,Pedcli.CAMPOCOM3,Pedcli.CAMPOCOM4,Pedcli.CAMPOCOM5,Pedcli.CAMPOCOM6,Pedcli.CG_PROY,Pedcli.VIA, " +
                "Pedcli.OBS1,Pedcli.OBS2,Pedcli.OBS3,Pedcli.OBS4,Pedcli.PROFORMA,Pedcli.MUESTRA,Pedcli.PREVORIG,Pedcli.DES_ART1,Pedcli.STOCKA,Pedcli.UNIDA,Pedcli.CANTENTA,Pedcli.CG_DEN,Pedcli.lote, " +
                "Pedcli.despacho,Pedcli.serie,Pedcli.cg_pallet,Pedcli.campana,Pedcli.OCOMPRA,Pedcli.CANTPED_ORI,Pedcli.CG_POSTA,Pedcli.DIRECC,Pedcli.ESTADO_IT,Pedcli.FE_AUT,Pedcli.USU_AUT,Pedcli.NOMRP, " +
                "Pedcli.CG_FORM_VENTAS,Pedcli.CONDVEN,Pedcli.CG_COS,Pedcli.ESTADO_LOGISTICA,Pedcli.CAMPOCOM7,Pedcli.CAMPOCOM8,Pedcli.CG_COND_ENTREGA,CAST( (CASE WHEN Pedidos.FLAG = 0 THEN 0 ELSE 1 END) AS BIT) " +
                "AS FLAG FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) where Pedcli.PEDIDO NOT IN" +
                "(select PEDIDO from Pedidos where TIPOO = 1) AND Programa.CG_ESTADO = 3  AND Pedcli.CANTPED > 0 AND Pedidos.TIPOO != 28");
            //string xSQL = string.Format("SELECT Pedcli.*, CAST( (CASE WHEN Pedidos.FLAG = 0 THEN 0 ELSE 1 END) AS BIT) AS FLAG " +
            //    "FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
            //    "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) where(pedidos.FLAG = 0 AND Programa.CG_ESTADO = 3 " +
            //    "AND Pedidos.CG_ORDF != 0 AND(Pedidos.TIPOO = 1)) " +
            //    "UNION " +
            //    "SELECT Pedcli.*, CAST( (CASE WHEN Pedidos.FLAG = 0 THEN 0 ELSE 1 END) AS BIT) AS FLAG " +
            //    "FROM((Pedcli INNER JOIN Programa ON Pedcli.PEDIDO = Programa.PEDIDO) " +
            //    "INNER JOIN Pedidos ON pedcli.PEDIDO = Pedidos.PEDIDO) " +
            //    "where Pedcli.PEDIDO NOT IN(select PEDIDO from Pedidos where TIPOO = 1) " +
            //    "AND Programa.CG_ESTADO = 3  AND Pedcli.CANTPED > 0 AND Pedidos.TIPOO != 28");
            return await base.DbSet.FromSqlRaw(xSQL).ToListAsync();

        }

        public async Task<IEnumerable<ModeloPedidosPendientes>> ObtenerPedidosPedientes()
        {
            string xSQL = string.Format("EXEC NET_PCP_PEDIDOS");
            return await base.Db.ModeloPedidosPendientes.FromSqlRaw(xSQL).ToListAsync();
        }

        public async Task<PedCliEncabezado> ObtenerPedidosEncabezado(int id)
        {
            var ped = await DbSet.FirstOrDefaultAsync(p=> p.PEDIDO == id);
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

        public async Task<PedCliEncabezado> ObtenerPedidosEncabezadoByNumOci(int numOci)
        {
            var pedidos = await DbSet.Where(p=> p.NUMOCI == numOci).ToListAsync();
            var pedEncabezado = new PedCliEncabezado();
            if (pedidos != null && pedidos.Count > 0) 
            {
                pedEncabezado.FE_MOV = pedidos[0].FE_PED;
                pedEncabezado.CONDICION_PAGO = pedidos[0].DPP;
                pedEncabezado.DIRENT = pedidos[0].DIRENT;
                pedEncabezado.CG_CLI = pedidos[0].CG_CLI;
                pedEncabezado.DES_CLI = pedidos[0].DES_CLI;
                pedEncabezado.FE_MOV = pedidos[0].FE_PED;
                pedEncabezado.PEDIDO = pedidos[0].PEDIDO;
                pedEncabezado.CG_TRANS = pedidos[0].CG_TRANS;
                pedEncabezado.CG_COND_ENTREGA = pedidos[0].CG_COND_ENTREGA;
                pedEncabezado.NUMOCI = pedidos[0].NUMOCI;
                pedEncabezado.MONEDA = pedidos[0].MONEDA;
                pedEncabezado.BONIFIC = pedidos[0].BONIFIC;
                pedEncabezado.ORCO = pedidos[0].ORCO;
                pedEncabezado.TC = (double)pedidos[0].VA_INDIC;

                await AgregarDireccionesEntrega(pedEncabezado);
                foreach (var item in pedidos)
                {
                    item.ESTADO = EstadoItem.Modificado;
                    pedEncabezado.Items.Add(item);
                }

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
            var genera = new Genera();
            try
            {
                if (list.Any(p=> p.ESTADO == EstadoItem.Agregado && p.NUMOCI == 0))
                {
                    await generaRepository.Reserva(NUMOCI);
                    genera = await generaRepository.Obtener(g => g.Id == NUMOCI).FirstOrDefaultAsync();
                }

                foreach (PedCli item in list)
                {
                    //agregar nueva oci: en la edidicon debe venir con la oci del grupo
                    if (item.NUMOCI == 0 && item.ESTADO == EstadoItem.Agregado)
                    {
                        item.NUMOCI = Convert.ToInt32(genera.VALOR1);
                    }
                    

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


                if (list.Any(p => p.ESTADO == EstadoItem.Agregado))
                {
                    await generaRepository.Libera(NUMOCI);
                }
                
                await SaveChanges();

                //lo ejecuta despues de obtener el numero de pedido
                await AsignarServicio(list);

                //establecer presupuesto como ganada y quitar de los pendientes
                await PresupuestoGanado(list);
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
                //COMO DETECTAR UN PEDIDO VIEJO QUE NO EXISTE EN SERVICIO
                if (item.CG_ART.StartsWith("0012") && item.PRESUPUESTOID > 0)
                {
                    //var servicio = Db.Servicios.Where(s => s.PRESUPUESTO == item.PRESUPUESTOID && s.PEDIDO == 0).FirstOrDefault();
                    var servicio = Db.Servicios.Where(s => s.SOLICITUD == item.SolicitudId && s.PEDIDO == 0).FirstOrDefault();
                    if (servicio is not null)
                    {
                        servicio.FECHA = DateTime.Now;
                        servicio.PEDIDO = item.PEDIDO;
                        servicio.OBSERV = string.IsNullOrEmpty(item.OBSERITEM) ? string.Empty : item.OBSERITEM;
                        servicio.OCOMPRA = string.IsNullOrEmpty(item.ORCO) ? string.Empty : item.ORCO;
                        Db.Entry(servicio).State = EntityState.Modified;
                        Db.Entry(servicio).Property(p => p.PEDIDO).IsModified = true;
                        Db.Entry(servicio).Property(p => p.FECHA).IsModified = true;
                        Db.Entry(servicio).Property(p => p.OCOMPRA).IsModified = true;
                        Db.Entry(servicio).Property(p => p.OBSERV).IsModified = true;
                        await Db.SaveChangesAsync(); 
                    }
                }
            }
        }
        private async Task PresupuestoGanado(List<PedCli> list)
        {

            var presupuestoUnicos = list.Where(p => p.PRESUPUESTOID > 0).DistinctBy(d => d.PRESUPUESTOID).ToList();

            foreach (var item in presupuestoUnicos)
            {
                var presupuesto = presupuestoRepository.Obtener(p => p.Id == item.PRESUPUESTOID).AsNoTracking().FirstOrDefault();

                if (presupuesto is not null)
                {   
                    presupuesto.TienePedido = true;
                    presupuesto.COLOR = "GANADA";
                    Db.Entry(presupuesto).State = EntityState.Modified;
                    Db.Entry(presupuesto).Property(p => p.TienePedido).IsModified = true;
                    Db.Entry(presupuesto).Property(p => p.COLOR).IsModified = true;
                    await Db.SaveChangesAsync();
                }
            }
        }
    }
}
