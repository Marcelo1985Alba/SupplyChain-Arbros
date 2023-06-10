using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class SolCotEmailRepository : Repository<SolCotEmail, decimal>
    {
        private readonly MailRepository _mailRepository;

        public SolCotEmailRepository(AppDbContext appDb, MailRepository mailRepository) : base(appDb)
        {
            _mailRepository = mailRepository;
        }

        internal async Task<List<SolCotEmail>> EnviarMail(List<SolCotEmail> mails)
        {
            
            await Db.AddRangeAsync(mails);
            await Db.SaveChangesAsync();

            foreach (var item in mails.GroupBy(m=> m.CG_PROVE).Select(s=> s.FirstOrDefault()).ToList())
            {
                await _mailRepository.EnviarCorreo(item.EMAIL, item.ASUNTO_EMAIL, item.MENSAJE_EMAIL);
            }

            return mails;
        }

        internal async Task<List<SolCotEmail>> GetWithNameProveBySugerenacias(List<Compra> sugerencias)
        {
            var idsCompras = sugerencias.Select(s => Convert.ToDecimal(s.Id)).ToArray();

            var lista = await DbSet.Where(s => idsCompras.Contains(s.REGISTRO_COMPRAS))
                 .ToListAsync();

            lista?.ForEach(s =>
            {
                s.Proveedor = Db.vProveedoresItris.Where(p => p.Id == s.CG_PROVE).Select(p=> p.DESCRIPCION).FirstOrDefault();

            });

            return lista;
        }
    }
}
