using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class HttpClientSinToken
    {
        public HttpClientSinToken(HttpClient client)
        {
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
