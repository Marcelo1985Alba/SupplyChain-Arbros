using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.RepositoryHttp;

public class RepositoryHttp : IRepositoryHttp
{
    private readonly HttpClient httpClient;

    public RepositoryHttp(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    private JsonSerializerOptions OpcionesPorDefecto => new() { PropertyNameCaseInsensitive = true };

    public AuthenticationHeaderValue? Authorization
    {
        get => httpClient.DefaultRequestHeaders.Authorization;
        set => httpClient.DefaultRequestHeaders.Authorization = value;
    }


    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        var responseHttp = await httpClient.GetAsync(url);
        return responseHttp;
    }

    public async Task<HttpResponseWrapper<T>> GetFromJsonAsync<T>(string url)
    {
        var responseHttp = await httpClient.GetAsync(url);
        if (responseHttp.IsSuccessStatusCode)
        {
            var respuesta = await DeserealizeResponse<T>(responseHttp, OpcionesPorDefecto);
            return new HttpResponseWrapper<T>(respuesta, responseHttp, false);
        }

        return new HttpResponseWrapper<T>(default, responseHttp, true);

        //responseHttp.Dispose();
        //httpClient.Dispose();
    }


    public async Task<HttpResponseWrapper<object>> PutAsJsonAsync<T>(string requestUri, T content)
    {
        var myContent = JsonSerializer.Serialize(content);
        StringContent stringContent = new(myContent, Encoding.UTF8, "application/json");
        var responseHttp = await httpClient.PutAsync(requestUri, stringContent);
        return new HttpResponseWrapper<object>(responseHttp, null, !responseHttp.IsSuccessStatusCode);
    }

    public async Task<HttpResponseWrapper<T>> PostAsJsonAsync<T>(string url, T sendContent)
    {
        var myContentJson = JsonSerializer.Serialize(sendContent);
        StringContent stringContent = new(myContentJson, Encoding.UTF8, "application/json");
        var responseHttp = await httpClient.PostAsync(url, stringContent);

        return await CreateWrapper<T>(responseHttp);
    }

    public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T enviar)
    {
        var enviarJSON = JsonSerializer.Serialize(enviar);
        var enviarContent = new StringContent(enviarJSON, Encoding.UTF8, "application/json");
        var responseHttp = await httpClient.PostAsync(url, enviarContent);
        if (responseHttp.IsSuccessStatusCode)
        {
            var response = await DeserealizeResponse<TResponse>(responseHttp, OpcionesPorDefecto);
            return new HttpResponseWrapper<TResponse>(response, responseHttp, false);
        }

        return new HttpResponseWrapper<TResponse>(default, responseHttp, true);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        var response = await httpClient.DeleteAsync(requestUri);
        //httpClient.Dispose();
        return response;
    }


    private async Task<T> DeserealizeResponse<T>(HttpResponseMessage httpResponseMessage,
        JsonSerializerOptions jsonSerializerOptions)
    {
        try
        {
            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(response, jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error en la deserialización de la respuesta HTTP: " + e.Message);
            return default;
        }
        finally
        {
            httpResponseMessage.Dispose();
        }
    }

    //private async Task<T> DeserealizeResponse<T>(HttpResponseMessage httpResponseMessage
    //    , JsonSerializerOptions jsonSerializerOptions)
    //{
    //    var response = await httpResponseMessage.Content.ReadAsStringAsync();
    //    return JsonSerializer.Deserialize<T>(response, jsonSerializerOptions);
    //}

    private async Task<HttpResponseWrapper<T>> CreateWrapper<T>(HttpResponseMessage responseHttp)
    {
        if (responseHttp.IsSuccessStatusCode)
        {
            var respuesta = await DeserealizeResponse<T>(responseHttp, OpcionesPorDefecto);
            return new HttpResponseWrapper<T>(respuesta, responseHttp, false);
        }

        return new HttpResponseWrapper<T>(default, responseHttp, true);
    }
}