using System.Net.Http;

namespace SupplyChain.Client.RepositoryHttp;

public class HttpResponseWrapper<T>
{
    public HttpResponseWrapper(T response, HttpResponseMessage httpResponseMessage, bool error)
    {
        Response = response;
        HttpResponseMessage = httpResponseMessage;
        Error = error;
    }

    public T Response { get; set; }
    public HttpResponseMessage HttpResponseMessage { get; set; }
    public bool Error { get; set; }
}