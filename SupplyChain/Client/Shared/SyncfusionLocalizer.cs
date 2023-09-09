using System.Resources;
using SupplyChain.Client.Resources;
using Syncfusion.Blazor;

public class SyncfusionLocalizer : ISyncfusionStringLocalizer
{
    // To get the locale key from mapped resources file

    public string GetText(string key)
    {
        return ResourceManager.GetString(key);
    }

    // To access the resource file and get the exact value for locale key
    public ResourceManager ResourceManager =>
        // Replace the ApplicationNamespace with your application name.
        Resources.ResourceManager;
}