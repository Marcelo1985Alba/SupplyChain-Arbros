using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SupplyChain.Server.Data.Repository
{
    public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : EntityBase, new()
    {
        protected readonly AppDbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        protected Repository(AppDbContext db)
        {
            Db = db;
            DbSet = Db.Set<TEntity>();
        }


        public virtual async Task<TEntity> ObtenerPorId(TId id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> ObtenerTodos()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> Obtener(Expression<Func<TEntity, bool>> filter,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query =  DbSet.AsNoTracking().Where(filter);

            includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return await query.ToListAsync();
        }

        public virtual async Task Agregar(TEntity entity)
        {
            DbSet.Add(entity);
            await SaveChanges();
        }

        public virtual async Task Actualizar(TEntity entity)
        {
            Db.Entry(entity).State = EntityState.Modified;
            await SaveChanges();
        }

        public virtual async Task Remover(TId id)
        {
            //Db.Entry(entity: new TEntity { Id = id }).State = EntityState.Deleted;
            var entity = await ObtenerPorId(id);
            Db.Remove(entity);
            await SaveChanges();
        }

        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }

        public async Task EnviarCsvDataCore()
        {
            string xSQL = string.Format("EXEC INTERFACE_DATACORE");
            await Db.Database.ExecuteSqlRawAsync(xSQL);
        }

        public async Task GenerarCsvQrByPedido(int pedido)
        {
            string xSQL = string.Format($"EXEC INTERFACE_IMPRESORAQR {pedido}");
            await Db.Database.ExecuteSqlRawAsync(xSQL);
        }

        public void Dispose()
        {
            Db?.Dispose();
        }
    }
}
