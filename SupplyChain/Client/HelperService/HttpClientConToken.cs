using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class HttpClientConToken
    {
        public HttpClientConToken(HttpClient client)
        {
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
