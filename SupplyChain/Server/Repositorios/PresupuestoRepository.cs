using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PresupuestoRepository : Repository<Presupuesto, int>
    {
        private readonly PrecioArticulosRepository _precioArticulosRepository;

        public PresupuestoRepository(AppDbContext appDbContext, PrecioArticulosRepository precioArticulosRepository) : base (appDbContext)
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
            }
            catch (Exception)
            {
                throw;
            }
  
        }

        internal async Task AgregarDatosFaltantes(Presupuesto presupuesto)
        {
            if (presupuesto.CG_CLI > 0 && string.IsNullOrEmpty(presupuesto.DES_CLI))
            {
                presupuesto.DES_CLI = (await Db.ClientesExternos
                    .FirstOrDefaultAsync(c => c.Id == presupuesto.CG_CLI.ToString())).DESCRIPCION;
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
            foreach (PresupuestoDetalle item in items)
            {
                if (item.Id < 0)
                {
                    item.Id = 0;
                    await Db.AddAsync(item);
                }
            }

            await Db.SaveChangesAsync();
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

        internal async Task AgregarActualizarDetalles(IList<PresupuestoDetalle> items)
        {
            await AgregarNuevosDetalles(items);
            await ActualizarDetalles(items);
        }

    }
}
