using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using System;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class PresupuestoAnteriorRepository : Repository<PresupuestoAnterior, int>
    {
        private readonly string REGISTRO = "REGPRES";
        private readonly string NUMERO = "PRESUP";
        private readonly GeneraRepository _generaRepository;

        public PresupuestoAnteriorRepository(AppDbContext appDbContext, GeneraRepository generaRepository) : base (appDbContext)
        {
            _generaRepository = generaRepository;
        }

        public override async Task Agregar(PresupuestoAnterior entity)
        {

            try
            {
                await _generaRepository.Reserva(NUMERO);
                await _generaRepository.Reserva(REGISTRO);

                entity.PRESUP = (int)(await _generaRepository.Obtener(g => g.Id == NUMERO).FirstOrDefaultAsync()).VALOR1;
                entity.Id = (int)(await _generaRepository.Obtener(g => g.Id == REGISTRO).FirstOrDefaultAsync()).VALOR1;

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

        internal async Task AgregarDatosFaltantes(PresupuestoAnterior presupuesto)
        {
            if (string.IsNullOrEmpty(presupuesto.DES_CLI))
            {
                presupuesto.DES_CLI = (await Db.Cliente.FirstOrDefaultAsync(c => c.Id == presupuesto.CG_CLI)).DES_CLI;
            }

            if (string.IsNullOrEmpty(presupuesto.DES_ART))
            {
                var prod = await Db.Prod.FirstOrDefaultAsync(c => c.Id.Trim() == presupuesto.CG_ART.Trim());
                if (prod != null)
                {
                    presupuesto.DES_ART = prod.DES_PROD.Trim();
                    presupuesto.UNID = prod.UNID.Trim();
                }
                
            }
        }
    }
}
