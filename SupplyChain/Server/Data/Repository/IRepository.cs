using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SupplyChain.Server.Data.Repository
{
    public interface IRepository<TEntity, TId> : IDisposable where TEntity : EntityBase
    {
        Task Agregar(TEntity entity);
        Task<TEntity> ObtenerPorId(TId id);
        Task<List<TEntity>> ObtenerTodos();
        Task Actualizar(TEntity entity);
        Task Remover(TId id);
        Task<IEnumerable<TEntity>> Obtener(Expression<Func<TEntity, bool>> filter,
            params Expression<Func<TEntity, object>>[] includes);
        Task<int> SaveChanges();
    }
}
