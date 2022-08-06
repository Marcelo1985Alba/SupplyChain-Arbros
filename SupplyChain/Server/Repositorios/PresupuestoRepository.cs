using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PresupuestoRepository : Repository<Presupuesto, int>
    {
        private readonly PrecioArticulosRepository _precioArticulosRepository;

        public PresupuestoRepository(AppDbContext appDbContext, PrecioArticulosRepository precioArticulosRepository
            ) : base (appDbContext)
        {
            _precioArticulosRepository = precioArticulosRepository;
        }

        public async Task<List<vPresupuestos>> GetForView()
        {
            return await Db.vPresupuestos.ToListAsync();
        }

        public override async Task Agregar(Presupuesto entity)
        {

            try
            {
                await base.Agregar(entity);
                await CerrarSolicitud(entity);
            }
            catch (Exception)
            {
                throw;
            }
  
        }


        public override async Task Actualizar(Presupuesto entity)
        {

            try
            {
                await base.Actualizar(entity);
                //cerrar solicitudes asociadas
                await CerrarSolicitud(entity);
            }
            catch (Exception)
            {
                throw;
            }

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
