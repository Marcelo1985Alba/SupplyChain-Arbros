using SupplyChain.Client.RepositoryHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService.Base
{
    public interface IService<TEntity, TId> : IDisposable where TEntity : class
    {
        Task<HttpResponseWrapper<List<TEntity>>> Get();
        Task<HttpResponseWrapper<TEntity>> GetById(TId id);
        Task<HttpResponseWrapper<TEntity>> Agregar(TEntity entity);
        Task<HttpResponseWrapper<object>> Actualizar(TId id, TEntity entity);
    }
}
