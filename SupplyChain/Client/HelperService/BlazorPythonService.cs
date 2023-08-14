using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;

namespace SupplyChain.Client.HelperService
{
    public class BlazorPythonService : IDisposable
    {
        private readonly HttpClient _httpClient;

        public BlazorPythonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public void Dispose()
        {
            ((IDisposable)_httpClient).Dispose();
        }

        public async Task<string> EjecutarFuncionEnPythonAsync(object data)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                //cambiar aca el puerto por 1234 y el nombre de la funcion en python por ejecutar_funcion
                var response = await _httpClient.PostAsync("http://localhost:1234/ejecutar_funcion", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResult>(responseContent);
                    return result?.Resultado;
                }
                else
                {
                    return "Error en la llamada a la API";
                }
            }
            catch (Exception ex)
            {
                return "Error en la llamada a la API: " + ex.Message;
            }
        }
    }

    public class ApiResult
    {
        public string Resultado { get; set; }
    }
}