using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SupplyChain.Client.RepositoryHttp
{
    public interface IRepositoryHttp
    {
        AuthenticationHeaderValue Authorization { get; set; }

        Task<HttpResponseWrapper<T>> GetFromJsonAsync<T>(string url);
        Task<HttpResponseWrapper<T>> PostAsJsonAsync<T>(string url, T sendContent);
        Task<HttpResponseWrapper<object>> PutAsJsonAsync<T>(string requestUri, T content);
    }
}
