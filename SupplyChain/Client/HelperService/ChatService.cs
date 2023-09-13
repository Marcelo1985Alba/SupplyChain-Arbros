using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Login;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class ChatService : BaseService<ChatMessage, long>
    {
        private const string API = "api/Chats";
        public ChatService(IRepositoryHttp httpClient) : base(httpClient, API)
        {

        }

        public async Task<HttpResponseWrapper<List<Usuario>>> GetUsersAsync()
        {
            return await http.GetFromJsonAsync<List<Usuario>>($"{API}/users");
        }

        public async Task<HttpResponseWrapper<List<ChatMessage>>> GetConversationAsync(string contactId)
        {
            return await http.GetFromJsonAsync<List<ChatMessage>>($"{API}/{contactId}");
        }

        public async Task<HttpResponseWrapper<List<ChatMessage>>> GetConversationNoViewAsync()
        {
            return await http.GetFromJsonAsync<List<ChatMessage>>($"{API}/NoView");
        }
        public async Task<HttpResponseWrapper<Usuario>> GetUserDetailsAsync(string userId)
        {
            return await http.GetFromJsonAsync<Usuario>($"{API}/users/{userId}");
        }
    }
}
