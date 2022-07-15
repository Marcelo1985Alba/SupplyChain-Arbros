using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class UsuariosRepository : Repository<Usuarios, int>
    {
        public UsuariosRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<Usuarios> GetByUserName(string usuario)
        {
            return await base.DbSet.Where(u => u.Usuario == usuario)
                .Include(c => c.Rol).FirstOrDefaultAsync();
        }

        public async Task<Usuarios> GetByUsernamePass(string usuario , string pass)
        {
            return await base.DbSet.Where(u => u.Usuario == usuario && u.Contras == pass)
                .Include(c => c.Rol).FirstOrDefaultAsync();
        }
    }
}
