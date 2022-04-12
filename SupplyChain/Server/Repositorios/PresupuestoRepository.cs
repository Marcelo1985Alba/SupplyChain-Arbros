using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PresupuestoRepository : Repository<Presupuesto, int>
    {
        private readonly string REGISTRO = "REGPRES";
        private readonly string NUMERO = "PRESUP";
        private readonly GeneraRepository _generaRepository;

        public PresupuestoRepository(AppDbContext appDbContext, GeneraRepository generaRepository) : base (appDbContext)
        {
            _generaRepository = generaRepository;
        }

        public override async Task Agregar(Presupuesto entity)
        {

            try
            {
                await _generaRepository.Reserva(NUMERO);
                await _generaRepository.Reserva(REGISTRO);

                entity.PRESUP = (int)(await _generaRepository.Obtener(g => g.CAMP3 == NUMERO).FirstOrDefaultAsync()).VALOR1;
                entity.REGISTRO = (int)(await _generaRepository.Obtener(g => g.CAMP3 == REGISTRO).FirstOrDefaultAsync()).VALOR1;

                await base.Agregar(entity);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await _generaRepository.Libera(NUMERO);
                await _generaRepository.Libera(REGISTRO);
            }
        }
        public async Task<bool> Existe(decimal id)
        {
            return await DbSet.AnyAsync(e => e.REGISTRO == id);
        }

        internal async Task AgregarDatosFaltantes(Presupuesto presupuesto)
        {
            if (string.IsNullOrEmpty(presupuesto.DES_CLI))
            {
                presupuesto.DES_CLI = (await Db.Cliente.FirstOrDefaultAsync(c => c.CG_CLI == presupuesto.CG_CLI)).DES_CLI;
            }

            if (string.IsNullOrEmpty(presupuesto.DES_ART))
            {
                var prod = await Db.Prod.FirstOrDefaultAsync(c => c.CG_PROD.Trim() == presupuesto.CG_ART.Trim());
                if (prod != null)
                {
                    presupuesto.DES_ART = prod.DES_PROD.Trim();
                    presupuesto.UNID = prod.UNID.Trim();
                }
                
            }
        }
    }
}
