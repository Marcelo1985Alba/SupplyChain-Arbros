using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class ModulosUsuarioRepository : Repository<ModulosUsuario, int>
    {
        public ModulosUsuarioRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task GuardarLista(List<ModulosUsuario> list)
        {
            try
            {
                var listaModulosUsuarios = await DbSet.AsNoTracking()
                    .Where(w=> w.UserId == list.First().UserId).ToListAsync();

                var listaAEliminar = (from lModulosUsuarioDb in listaModulosUsuarios
                                     join listNueva in list on new { lModulosUsuarioDb.ModuloId, lModulosUsuarioDb.UserId }
                                     equals new { listNueva.ModuloId, listNueva.UserId }
                                     select new ModulosUsuario()
                                     {
                                         Id = lModulosUsuarioDb.Id,
                                         ModuloId = listNueva.ModuloId,
                                         UserId = listNueva.UserId
                                     }).ToList();
                DbSet.RemoveRange(listaAEliminar);
                await Db.SaveChangesAsync();



                await base.DbSet.AddRangeAsync(list);
                await Db.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
            }
        }
    }
}
