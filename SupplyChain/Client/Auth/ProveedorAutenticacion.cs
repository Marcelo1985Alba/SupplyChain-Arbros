using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;

namespace SupplyChain.Client.Auth;

public class ProveedorAutenticacion : AuthenticationStateProvider, ILoginService
{
    public static readonly string USER = "Usuario";
    private readonly HttpClient httpClient;
    private readonly IJSRuntime js;

    public ProveedorAutenticacion(IJSRuntime js, HttpClient httpClient)
    {
        this.js = js;
        this.httpClient = httpClient;
    }

    private AuthenticationState Anonimo =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public Usuarios UsuarioLogin { get; set; }

    public async Task Login(Usuarios userToken)
    {
        UsuarioLogin = userToken;
        await js.SetInSessionStorage(USER, userToken.Usuario);

        //await js.SetInLocalStorage(EXPIRATIONTOKENKEY, userToken.Expiration.ToString());
        var authState = ConstruirAuthenticationState(userToken);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public async Task Logout()
    {
        UsuarioLogin = null;
        await Limpiar();
        NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var usuario = await js.GetFromSessionStorage("Usuario");

        if (string.IsNullOrEmpty(usuario)) return Anonimo;
        if (UsuarioLogin is null || string.IsNullOrEmpty(UsuarioLogin.Nombre))
            UsuarioLogin = await httpClient.GetFromJsonAsync<Usuarios>($"api/Usuarios/{usuario}");

        return ConstruirAuthenticationState(UsuarioLogin);
    }

    public AuthenticationState ConstruirAuthenticationState(Usuarios usuario)
    {
        //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        //return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        //todo: OBTENER ROL DEL USUARIO

        var userAuth = new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.Name, usuario.Usuario),
            new(ClaimTypes.Role, usuario.Rol.Descripcion)
        }, "Arbros");

        return new AuthenticationState(new ClaimsPrincipal(userAuth));
    }

    private async Task Limpiar()
    {
        await js.RemoveSessionItem(USER);
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}