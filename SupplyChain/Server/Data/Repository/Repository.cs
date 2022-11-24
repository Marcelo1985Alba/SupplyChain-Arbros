using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SupplyChain.Server.Data.Repository
{
    public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : EntityBase<TId>, new()
    {
        protected readonly AppDbContext Db;
        protected readonly DbSet<TEntity> DbSet;
        public readonly DatabaseFacade Database;

        protected Repository(AppDbContext db)
        {
            Db = db;
            DbSet = Db.Set<TEntity>();
            Database = Db.Database;

        }

        private IQueryable<TEntity> GetAsQueryable(Expression<Func<TEntity, bool>> filter, int take,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var result = DbSet.Where(filter);
            includes.Aggregate(result, (current, includeProperty) => current.Include(includeProperty));
            
            return result;
        }
        public virtual async Task<bool> Existe(TId id)
        {
            var entity = await ObtenerPorId(id);
            return entity != null;
        }

        public virtual async Task<bool> Existe(params object[] keyValues)
        {
            var entity = await DbSet.FindAsync(keyValues);
            return entity != null;
        }
        public virtual async Task<TEntity> ObtenerPorId(TId id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> ObtenerTodos()
        {
            return await DbSet.ToListAsync();
        }

        public virtual IQueryable<TEntity> ObtenerTodosQueryable()
        {
            return DbSet.AsQueryable();
        }

        public IQueryable<TEntity> Obtener(Expression<Func<TEntity, bool>> filter, int take = 0,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool ascending = false, params Expression<Func<TEntity, object>>[] includes )
        {
            var query = GetAsQueryable(filter, take, includes);
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (take > 0) query = query.Take(take);

            return query;
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

        public virtual async Task RemoverList(List<TId> lista)
        {
            //Db.Entry(entity: new TEntity { Id = id }).State = EntityState.Deleted;
            foreach (var item in lista)
            {
                var entity = await ObtenerPorId(item);
                Db.RemoveRange(entity);
            }
            
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

        public async Task BeginTransaction()
        {
            await Db.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await Db.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransaction()
        {
            await Db.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            Db?.Dispose();
        }

    }
}
