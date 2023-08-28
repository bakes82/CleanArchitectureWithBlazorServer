using Microsoft.JSInterop;

namespace Blazor.Server.UI.Services.JsInterop;

public class OpenSeadragon
{
    private readonly IJSRuntime _jsRuntime;

    public OpenSeadragon(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask Open(string url)
    {
        string target = "openseadragon";
        return _jsRuntime.InvokeVoidAsync(JsInteropConstants.ShowOpenSeadragon, target, url);
    }
}