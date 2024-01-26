using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PresupuestoRepository : Repository<Presupuesto, int>
    {
        private readonly PrecioArticulosRepository _precioArticulosRepository;
        private readonly SolicitudRepository _solicitudRepository;

        public PresupuestoRepository(AppDbContext appDbContext, PrecioArticulosRepository precioArticulosRepository,
            SolicitudRepository solicitudRepository
            ) : base (appDbContext)
        {
            _precioArticulosRepository = precioArticulosRepository;
            this._solicitudRepository = solicitudRepository;
        }

        public async Task<List<vPresupuestos>> GetForView(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            if (tipoFiltro == TipoFiltro.Pendientes)
            {
                return await Db.vPresupuestos.Where(p=> string.IsNullOrEmpty(p.COLOR) || p.COLOR.Contains("PENDIENTE")).ToListAsync();
            }

            if (tipoFiltro == TipoFiltro.NoPendientes)
            {
                return await Db.vPresupuestos.Where(p => !p.TIENEPEDIDO && p.COLOR.Contains("PERDIDA")).ToListAsync();
            }
            return await Db.vPresupuestos.ToListAsync();
        }


        internal async Task<bool> TienePedido(int presupuestoId)
        {
            return await Db.PedCli.AnyAsync(p => p.PRESUPUESTOID == presupuestoId);
        }

        internal async Task DesvincularSolicitud(Presupuesto presupuesto)
        {
            await presupuesto.Items.ForEachAsync(async item =>
            {
                var sol = await Db.Solicitudes.FirstOrDefaultAsync(s=> s.Id == item.SOLICITUDID);
                if (sol is not null)
                {
                    sol.TienePresupuesto = false;
                    Db.Entry(sol).State = EntityState.Modified;
                    Db.Entry(sol).Property(s=> s.TienePresupuesto).IsModified = true;
                    await Db.SaveChangesAsync();
                }
            });
        }

        public override async Task Agregar(Presupuesto entity)
        {

            try
            {

                if (string.IsNullOrEmpty(entity.DIRENT))
                {
                    entity.DIRENT = string.Empty;
                }
                await base.Agregar(entity);
                await CerrarSolicitud(entity);

                await AsignarServicio(entity);
            }
            catch (Exception)
            {
                throw;
            }
  
        }

        public async Task<IEnumerable<Presupuesto>> EnviarComentario(int id, string comentario)
        {
            string xSql = $"UPDATE PRESUPUESTO_ENCABEZADO SET COMENTARIO = '{comentario}' WHERE ID ={id}";
            await base.Database.ExecuteSqlRawAsync(xSql);

            return await DbSet.Where(p => p.Id == id).ToListAsync();
        }


        private async Task AsignarServicio(Presupuesto entity)
        {
            //COMO DETECTAR UN PRESUPUESTO VIEJO QUE NO EXISTE EN SERVICIO
            foreach (var item in entity.Items.Where(p => p.CG_ART.StartsWith("0012")))
            {

                if (item.CG_ART.StartsWith("0012") && item.SOLICITUDID > 0)
                {
                    var servicio = Db.Servicios.Where(s => s.SOLICITUD == item.SOLICITUDID).FirstOrDefault();
                    if (servicio is not null )
                    {
                        servicio.FECHA = DateTime.Now;
                        servicio.PRESUPUESTO = item.PRESUPUESTOID;
                        Db.Entry(servicio).State = EntityState.Modified;
                        Db.Entry(servicio).Property(p => p.PRESUPUESTO).IsModified = true;
                        Db.Entry(servicio).Property(p => p.FECHA).IsModified = true;
                        await Db.SaveChangesAsync(); 
                    }
                }
            }
        }

        public override async Task Actualizar(Presupuesto entity)
        {

            try
            {
                if (string.IsNullOrEmpty(entity.DIRENT))
                {
                    entity.DIRENT = string.Empty;
                }
                await base.Actualizar(entity);
                //cerrar solicitudes asociadas
                await CerrarSolicitud(entity);
            }
            catch (Exception)
            {
                throw;
            }

        }


        public async Task<IEnumerable<Presupuesto>> ActualizarColor(int id, string color)
        { 
            string xSQL= $"UPDATE PRESUPUESTO_ENCABEZADO SET COLOR='{color}' WHERE ID = {id}";
            await base.Database.ExecuteSqlRawAsync(xSQL);

            return await DbSet.Where(p=> p.Id == id).ToListAsync();
        }

        public async Task<IEnumerable<Presupuesto>> EnviarMotivos(int id, string motivo)
        {
            string xSQL = $"UPDATE PRESUPUESTO_ENCABEZADO SET MOTIVO ='{motivo}' WHERE ID = {id}";
            await base.Database.ExecuteSqlRawAsync(xSQL);

            return await DbSet.Where(m=> m.Id == id).ToListAsync();
        }


        private async Task CerrarSolicitud(Presupuesto entity)
        {
            foreach (var item in entity.Items)
            {
                if (item.SOLICITUDID > 0)
                {
                    await Db.Database.ExecuteSqlRawAsync($"UPDATE SOLICITUD SET TIENEPRESUPUESTO = 1 WHERE ID = {item.SOLICITUDID}");
                }
            }
        }

        internal async Task AgregarDatosFaltantes(Presupuesto presupuesto)
        {
            if (presupuesto.CG_CLI > 0 && string.IsNullOrEmpty(presupuesto.DES_CLI))
            {
                presupuesto.DES_CLI = (await Db.ClientesExternos
                    .FirstOrDefaultAsync(c => c.CG_CLI == presupuesto.CG_CLI.ToString())).DESCRIPCION;
            }


            if (presupuesto.Items.Count > 0)
            {
                foreach (PresupuestoDetalle item in presupuesto.Items)
                {
                    if (string.IsNullOrEmpty(item.DES_ART))
                    {
                        var precio = await _precioArticulosRepository.ObtenerPorId(item.CG_ART.Trim());
                        if (precio != null)
                        {
                            item.DES_ART = precio.Descripcion.Trim();
                            //presupuesto.UNID = precio..Trim();
                        }

                    }

                    if (item.SOLICITUDID > 0)
                    {
                        var solicitud = await _solicitudRepository.ObtenerPorId(item.SOLICITUDID);
                        if (solicitud != null)
                        {
                            item.Solicitud = solicitud;
                        }
                    }
                } 
            }

            
        }

        /// <summary>
        /// Agrega items nuevos a un presupuesto existente
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        internal async Task AgregarNuevosDetalles(IList<PresupuestoDetalle> items)
        {
            var agregar = items.Any(i => i.Estado == Shared.Enum.EstadoItem.Agregado && i.Id < 0);
            
            if (agregar)
            {
                var itemsAgregar = items.Where(i => i.Estado == Shared.Enum.EstadoItem.Agregado && i.Id < 0).ToArray();
                foreach (var item in itemsAgregar)
                {
                    item.Id = 0;
                }
                await Db.AddRangeAsync(itemsAgregar);
                await Db.SaveChangesAsync();


            }

            
        }

        internal async Task ActualizarDetalles(IList<PresupuestoDetalle> items)
        {
            foreach (PresupuestoDetalle item in items)
            {
                if (item.Id > 0)
                {
                    Db.Entry(item).State = EntityState.Modified;
                }
            }

            await Db.SaveChangesAsync();
        }

        internal async Task AgregarEliminarActualizarDetalles(IList<PresupuestoDetalle> items)
        {
            await AgregarNuevosDetalles(items);
            await ActualizarDetalles(items);
            await RemoverDetalles(items);
        }

        internal async Task RemoverDetalles(IList<PresupuestoDetalle> items)
        {
            var elimina = items.Any(i=> i.Estado == Shared.Enum.EstadoItem.Eliminado && i.Id > 0 );

            if (elimina)
            {
                var itemsEliminar = items.Where(i => i.Estado == Shared.Enum.EstadoItem.Eliminado && i.Id > 0).ToArray();
                Db.RemoveRange(itemsEliminar);
                await SaveChanges();
            }
            
        }

        internal async Task ActualizarCalculoConPresupuestoByIdCalculo(int id)
        {
            await Db.Database.ExecuteSqlRawAsync($"Exec Solicitud_ActualizaPresupuesto {id}");
        }

    }
}
