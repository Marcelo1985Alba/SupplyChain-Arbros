﻿using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService.Base
{
    public abstract class BaseService<TEntity, TId> : IService<TEntity, TId> where TEntity : EntityBase<TId>, new()
    {
        public readonly IRepositoryHttp http;
        private readonly string api;

        public BaseService(IRepositoryHttp Http, string api)
        {
            http = Http;
            this.api = api;
        }

        public virtual async Task<HttpResponseWrapper<List<TEntity>>> Get()
        {
            return await http.GetFromJsonAsync<List<TEntity>>(api);
        }

        public virtual async Task<HttpResponseWrapper<List<TEntity>>> GetByFilter(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            return await http.GetFromJsonAsync<List<TEntity>>($"{api}/ByFilter/{tipoFiltro}");
        }

        public async Task<HttpResponseWrapper<TEntity>> GetById(TId id)
        {
            return await http.GetFromJsonAsync<TEntity>($"{api}/{id}");
        }


        public async Task<HttpResponseWrapper<object>> Actualizar(TId id, TEntity entity)
        {
            return await http.PutAsJsonAsync($"{api}/{id}", entity);
        }

        public async Task<HttpResponseWrapper<TEntity>> Agregar(TEntity entity)
        {
            return await http.PostAsJsonAsync($"{api}", entity);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        
    }
}
