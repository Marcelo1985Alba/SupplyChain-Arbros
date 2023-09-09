using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared.Login;
using SupplyChain.Shared.Models;

namespace SupplyChain.Client.Auth;

public class ProveedorAutenticacionJWT : AuthenticationStateProvider, ILoginServiceJWT
{
    public static readonly string TOKEN_KEY = "TOKEN_KEY";
    public static readonly string EXPIRATION_KEY = "EXPIRATION_KEY";
    public static readonly string FOTO_KEY = "FOTO_KEY";
    private readonly HttpClient httpClient;
    private readonly IJSRuntime js;
    private readonly IRepositoryHttp Repositorio;

    public ProveedorAutenticacionJWT(IJSRuntime js, IRepositoryHttp repositorio, HttpClient httpClient)
    {
        this.js = js;
        Repositorio = repositorio;
        this.httpClient = httpClient;
    }

    public Usuarios UsuarioLogin { get; set; }

    private AuthenticationState Anonimo =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public async Task ManejarRenovacionToken()
    {
        var tiempoExpiracionString = await js.GetFromSessionStorage(EXPIRATION_KEY);
        DateTime tiempoExpiracion;

        if (DateTime.TryParse(tiempoExpiracionString, out tiempoExpiracion))
        {
            if (TokenExpirado(tiempoExpiracion)) await Logout();

            if (DebeRenovarToken(tiempoExpiracion))
            {
                var token = await js.GetFromSessionStorage(TOKEN_KEY);
                var nuevoToken = await RenovarToken(token);
                var authState = ConstruirAuthenticationState(nuevoToken);
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
            }
        }
    }

    public async Task Login(UserToken userToken)
    {
        await js.SetInSessionStorage(TOKEN_KEY, userToken.Token);
        await js.SetInSessionStorage(EXPIRATION_KEY, userToken.Expiration.ToString());
        await js.SetInSessionStorage(FOTO_KEY,
            userToken.Foto == null || userToken.Foto.Length == 0
                ? string.Empty
                : Convert.ToBase64String(userToken.Foto));
        var authState = ConstruirAuthenticationState(userToken.Token);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public async Task Logout()
    {
        //var response = await Repositorio.get<int>("api/Cuentas/Logout");
        await Limpiar();
        NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await js.GetFromSessionStorage(TOKEN_KEY);

        if (string.IsNullOrEmpty(token)) return Anonimo;

        var tiempoExpiracionString = await js.GetFromSessionStorage(EXPIRATION_KEY);
        DateTime tiempoExpiracion;
        if (DateTime.TryParse(tiempoExpiracionString, out tiempoExpiracion))
        {
            if (TokenExpirado(tiempoExpiracion))
            {
                await Limpiar();
                return Anonimo;
            }

            if (DebeRenovarToken(tiempoExpiracion)) token = await RenovarToken(token);
        }

        return ConstruirAuthenticationState(token);
    }

    private async Task<string> RenovarToken(string token)
    {
        Console.WriteLine("Renovando token...");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var response = await Repositorio.GetFromJsonAsync<UserToken>("api/Cuentas/renovarToken");
        var nuevoToken = response.Response;
        await js.SetInSessionStorage(TOKEN_KEY, nuevoToken.Token);
        await js.SetInSessionStorage(EXPIRATION_KEY, nuevoToken.Expiration.ToString());
        await js.SetInSessionStorage(FOTO_KEY, nuevoToken.Foto == null || nuevoToken.Foto.Length == 0
            ? string.Empty
            : nuevoToken.Foto.ToString());
        return nuevoToken.Token;
    }

    private bool TokenExpirado(DateTime tiempoExpira)
    {
        var isExpirado = tiempoExpira <= DateTime.Now;
        return isExpirado;
    }

    private bool DebeRenovarToken(DateTime tiempoExpira)
    {
        var tiempoEx = TimeSpan.FromMinutes(45);
        var tiempoSubs = tiempoExpira.Subtract(DateTime.Now);
        var t = tiempoSubs <= tiempoEx;

        return t;
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

    private async Task Limpiar()
    {
        await js.RemoveSessionItem(TOKEN_KEY);
        await js.RemoveSessionItem(EXPIRATION_KEY);
        await js.RemoveSessionItem(FOTO_KEY);

        httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

        if (roles != null)
        {
            if (roles.ToString().Trim().StartsWith("["))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                foreach (var parsedRole in parsedRoles) claims.Add(new Claim(ClaimTypes.Role, parsedRole));
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
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
}