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

namespace SupplyChain.Client.Auth
{
    public class ProveedorAutenticacion : AuthenticationStateProvider, ILoginService
    {
        private readonly IJSRuntime js;
        private readonly HttpClient httpClient;
        public static readonly string USER = "Usuario";
        public ProveedorAutenticacion(IJSRuntime js, HttpClient httpClient)
        {
            this.js = js;
            this.httpClient = httpClient;
        }

        private AuthenticationState Anonimo =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var usuario = await js.GetFromSessionStorage("Usuario");

            if (string.IsNullOrEmpty(usuario))
            {
                return Anonimo;
            }

            return ConstruirAuthenticationState(usuario);
        }

        public AuthenticationState ConstruirAuthenticationState(string usuario)
        {
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            //return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
            var userAuth = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario),
                new Claim(ClaimTypes.Role, "Admin")
            }, "testing");

            return new AuthenticationState(new ClaimsPrincipal(userAuth));
        }

        public async Task Login(string userToken)
        {
            await js.SetInSessionStorage(USER, userToken);
            //await js.SetInLocalStorage(EXPIRATIONTOKENKEY, userToken.Expiration.ToString());
            var authState = ConstruirAuthenticationState(userToken);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await Limpiar();
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }

        private async Task Limpiar()
        {
            await js.RemoveSessionItem(USER);
            httpClient.DefaultRequestHeaders.Authorization = null;
        }


    }
}
