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

        public PresupuestoRepository(AppDbContext appDbContext) : base (appDbContext)
        {
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
            if (string.IsNullOrEmpty(presupuesto.DES_CLI))
            {
                presupuesto.DES_CLI = (await Db.ClientesExternos
                    .FirstOrDefaultAsync(c => c.Id == presupuesto.CG_CLI.ToString())).DESCRIPCION;
            }

            if (string.IsNullOrEmpty(presupuesto.DES_ART))
            {
                //var prod = await Db.Prod.FirstOrDefaultAsync(c => c.Id.Trim() == presupuesto.Items.CG_ART.Trim());
                //if (prod != null)
                //{
                //    presupuesto.DES_ART = prod.DES_PROD.Trim();
                //    presupuesto.UNID = prod.UNID.Trim();
                //}
                
            }
        }
    }
}
