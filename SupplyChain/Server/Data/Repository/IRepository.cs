using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SupplyChain.Server.Data.Repository
{
    public interface IRepository<TEntity, TId> : IDisposable where TEntity : EntityBase<TId>
    {
        Task Agregar(TEntity entity);
        Task<bool> AgregarList(IEnumerable<TEntity> entity);
        Task<TEntity> ObtenerPorId(TId id);
        Task<List<TEntity>> ObtenerTodos();
        Task Actualizar(TEntity entity);
        Task Remover(TId id);
        IQueryable<TEntity> Obtener(Expression<Func<TEntity, bool>> filter, int take = 0,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool ascending = false, params Expression<Func<TEntity, object>>[] includes);
        Task<int> SaveChanges();
    }
}
