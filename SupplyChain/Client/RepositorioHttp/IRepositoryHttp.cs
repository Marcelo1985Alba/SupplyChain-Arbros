using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SupplyChain.Client.RepositoryHttp
{
    /// <summary>
    /// Enviar solicitudes HTTP y recibir respuestas de un servidor
    /// </summary>
    public interface IRepositoryHttp
    {
        AuthenticationHeaderValue? Authorization { get; set; }

        Task<HttpResponseWrapper<T>> GetFromJsonAsync<T>(string url);
        Task<HttpResponseMessage> GetAsync(string url);
        //Task<HttpResponseWrapper<TResponse>> Get<T, TResponse>(string url);
        Task<HttpResponseWrapper<T>> PostAsJsonAsync<T>(string url, T sendContent);
        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T enviar);
        Task<HttpResponseWrapper<object>> PutAsJsonAsync<T>(string requestUri, T content);
        Task<HttpResponseMessage> DeleteAsync(string requestUri);
    }
}
