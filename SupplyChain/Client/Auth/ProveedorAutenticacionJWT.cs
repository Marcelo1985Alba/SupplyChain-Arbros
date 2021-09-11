using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService;
using System.Net.Http.Headers;
using SupplyChain.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace SupplyChain.Client.Auth
{
    public class ProveedorAutenticacionJWT : AuthenticationStateProvider, ILoginServiceJWT
    {
        private readonly IJSRuntime js;
        private readonly HttpClient httpClient;
        public static readonly string TOKEN_KEY = "TOKEN_KEY";
        public Usuarios UsuarioLogin { get; set; }
        public ProveedorAutenticacionJWT(IJSRuntime js, HttpClient httpClient)
        {
            this.js = js;
            this.httpClient = httpClient;
        }

        private AuthenticationState Anonimo =>
            new(new ClaimsPrincipal(new ClaimsIdentity()));

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await js.GetFromSessionStorage(TOKEN_KEY);

            if (string.IsNullOrEmpty(token))
            {
                return Anonimo;
            }
            
            return ConstruirAuthenticationState(token);
        }

        public AuthenticationState ConstruirAuthenticationState(string token)
        {
            //token en cada peticion
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }


        //public async Task Logout()
        //{
        //    UsuarioLogin = null;
        //    await Limpiar();
        //    NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        //}

        //private async Task Limpiar()
        //{
        //    await js.RemoveSessionItem(USER);
        //    httpClient.DefaultRequestHeaders.Authorization = null;
        //}

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public async Task Login(string token)
        {
            await js.SetInSessionStorage(TOKEN_KEY, token);
            var authState = ConstruirAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await js.RemoveSessionItem(TOKEN_KEY);
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }
    }
}
