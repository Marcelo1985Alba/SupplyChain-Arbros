using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace SupplyChain.Client.HelperService;

public static class JSRuntimeExtensionMethods
{
    public static ValueTask<object> SetInLocalStorage(this IJSRuntime js, string key, string content)
    {
        return js.InvokeAsync<object>(
            "localStorage.setItem",
            key, content
        );
    }

    public static ValueTask<string> GetFromLocalStorage(this IJSRuntime js, string key)
    {
        return js.InvokeAsync<string>(
            "localStorage.getItem",
            key
        );
    }

    public static ValueTask<object> RemoveItem(this IJSRuntime js, string key)
    {
        return js.InvokeAsync<object>(
            "localStorage.removeItem",
            key);
    }

    public static ValueTask<object> SetInSessionStorage(this IJSRuntime js, string key, string content)
    {
        return js.InvokeAsync<object>(
            "sessionStorage.setItem",
            key, content
        );
    }

    public static ValueTask<string> GetFromSessionStorage(this IJSRuntime js, string key)
    {
        return js.InvokeAsync<string>(
            "sessionStorage.getItem",
            key
        );
    }

    public static ValueTask<object> RemoveSessionItem(this IJSRuntime js, string key)
    {
        return js.InvokeAsync<object>(
            "sessionStorage.removeItem",
            key);
    }
}